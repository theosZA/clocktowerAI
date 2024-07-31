using Clocktower.Agent;
using Clocktower.Events;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    internal class Kills
    {
        public Kills(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.scriptCharacters = scriptCharacters;
        }

        public async Task Execute(Player player)
        {
            player.Tokens.Add(Token.Executed, player);
            grimoire.MostRecentlyExecutedPlayerToDie = player;
            await DayKill(player, execution: true);
        }

        public async Task DayKill(Player player, Player? killer = null, bool execution = false)
        {
            await HandleDayDeath(player, killer, execution);
            player.Kill();
        }

        public async Task NightKill(Player player, Player? killer)
        {
            if (player.HasHealthyAbility(Character.Mayor)
                && !(killer?.CharacterType == CharacterType.Demon && player.ProtectedFromDemonKill)
                && killer?.Character != Character.Assassin)
            {
                player = await storyteller.GetMayorBounce(player, killer, grimoire.Players);
            }

            if (killer?.CharacterType == CharacterType.Demon && player.ProtectedFromDemonKill)
            {
                return;
            }

            await HandleNightDeath(player, killer);
            player.Tokens.Add(Token.DiedAtNight, killer ?? player);
        }

        private async Task HandleDayDeath(Player dyingPlayer, Player? killer, bool execution)
        {
            await ProcessDayDeathTriggers(dyingPlayer, execution);
            await ProcessDeathTriggers(dyingPlayer, killer);
            grimoire.ClearTokensOnPlayerDeath(dyingPlayer);
        }

        private async Task HandleNightDeath(Player dyingPlayer, Player? killer)
        {
            await ProcessNightDeathTriggers(dyingPlayer, killer);
            await ProcessDeathTriggers(dyingPlayer, killer);
            grimoire.ClearTokensOnPlayerDeath(dyingPlayer);
        }

        private async Task ProcessDayDeathTriggers(Player dyingPlayer, bool execution)
        {
            // Saint
            if (execution && dyingPlayer.HasHealthyAbility(Character.Saint))
            {
                grimoire.EndGame(dyingPlayer.Alignment == Alignment.Good ? Alignment.Evil : Alignment.Good);
            }

            // Godfather
            if (dyingPlayer.CharacterType == CharacterType.Outsider)
            {
                foreach (var godfather in grimoire.PlayersForWhomWeShouldRunAbility(Character.Godfather))
                {
                    godfather.Tokens.Add(Token.GodfatherKillsTonight, dyingPlayer);
                }
            }

            // Cannibal
            if (execution)
            {
                foreach (var cannibal in grimoire.PlayersForWhomWeShouldRunAbility(Character.Cannibal))
                {
                    foreach (var player in grimoire.Players)
                    {
                        player.Tokens.Remove(Token.CannibalEaten, cannibal);
                    }
                    dyingPlayer.Tokens.Add(Token.CannibalEaten, cannibal);
                    cannibal.Tokens.Add(Token.CannibalFirstNightWithAbility, cannibal);
                    cannibal.Tokens.Remove(Token.CannibalPoisoned);
                    if (dyingPlayer.Alignment == Alignment.Evil || dyingPlayer.RealCharacter == Character.Drunk)
                    {
                        cannibal.Tokens.Add(Token.CannibalPoisoned, cannibal);
                        var fakeCannibalAbility = await storyteller.ChooseFakeCannibalAbility(cannibal, dyingPlayer, scriptCharacters.Where(character => character.Alignment() == Alignment.Good));
                        cannibal.CannibalAbility = fakeCannibalAbility;
                    }
                    else
                    {
                        cannibal.CannibalAbility = dyingPlayer.RealCharacter;
                        if (cannibal.CannibalAbility == Character.Fortune_Teller)
                        {
                            await new AssignFortuneTellerRedHerring(storyteller, grimoire).AssignRedHerring(fortuneTeller: cannibal);
                        }
                    }
                }
            }
        }

        private async Task ProcessNightDeathTriggers(Player dyingPlayer, Player? killer)
        {
            // Ravenkeeper
            if (dyingPlayer.ShouldRunAbility(Character.Ravenkeeper))
            {
                var target = await dyingPlayer.Agent.RequestChoiceFromRavenkeeper(grimoire.Players);
                var character = await GetCharacterSeenByRavenkeeper(dyingPlayer, target);
                storyteller.ChoiceFromRavenkeeper(dyingPlayer, target, character);
                await dyingPlayer.Agent.NotifyRavenkeeper(target, character);
            }
        }

        private async Task ProcessDeathTriggers(Player dyingPlayer, Player? killer)
        {
            // Scarlet Woman
            bool scarletWomanTriggered = false;
            if (dyingPlayer.CharacterType == CharacterType.Demon && grimoire.Players.Count(player => player.Alive) >= 5)
            {
                var scarletWoman = grimoire.PlayersWithHealthyAbility(Character.Scarlet_Woman).FirstOrDefault(); // shouldn't be more than 1 Scarlet Woman
                if (scarletWoman != null)
                {
                    scarletWomanTriggered = true;
                    storyteller.ScarletWomanTrigger(dyingPlayer, scarletWoman);
                    grimoire.ChangeCharacter(scarletWoman, dyingPlayer.Character);
                }
            }
            // Imp star pass (excluding Scarlet Woman)
            if (!scarletWomanTriggered && dyingPlayer.Character == Character.Imp && killer == dyingPlayer)
            {
                var newImp = await GetNewImp();
                if (newImp != null)
                {
                    grimoire.ChangeCharacter(newImp, Character.Imp);
                    storyteller.AssignCharacter(newImp);
                }
            }
            // Sweetheart
            if (dyingPlayer.Character == Character.Sweetheart)
            {
                var sweetheartDrunk = await storyteller.GetSweetheartDrunk(grimoire.Players);
                sweetheartDrunk.Tokens.Add(Token.SweetheartDrunk, dyingPlayer);
            }
        }

        private async Task<Player?> GetNewImp()
        {
            var aliveMinions = grimoire.Players.Where(player => player.Alive && player.CharacterType == CharacterType.Minion).ToList();
            return aliveMinions.Count switch
            {
                0 => null,  // Nobody to star-pass to!
                1 => aliveMinions[0],
                _ => await storyteller.GetNewImp(aliveMinions),
            };
        }

        private async Task<Character> GetCharacterSeenByRavenkeeper(Player ravenkeeper, Player target)
        {
            if (ravenkeeper.DrunkOrPoisoned)
            {   // Any on-script character is possible.
                return await GetCharacterSeenByRavenkeeperFromList(ravenkeeper, target, scriptCharacters);
            }

            List<Character> characters = new() { target.RealCharacter };

            if (target.CanRegisterAsDemon && target.CharacterType != CharacterType.Demon)
            {
                foreach (var demon in scriptCharacters.OfCharacterType(CharacterType.Demon))
                {
                    characters.Add(demon);
                }
            }
            if (target.CanRegisterAsMinion && target.CharacterType != CharacterType.Minion)
            {
                foreach (var minion in scriptCharacters.OfCharacterType(CharacterType.Minion))
                {
                    characters.Add(minion);
                }
            }
            if (target.CanRegisterAsOutsider && target.CharacterType != CharacterType.Outsider)
            {
                foreach (var outsider in scriptCharacters.OfCharacterType(CharacterType.Outsider))
                {
                    characters.Add(outsider);
                }
            }
            if (target.CanRegisterAsTownsfolk && target.CharacterType != CharacterType.Townsfolk)
            {
                foreach (var townsfolk in scriptCharacters.OfCharacterType(CharacterType.Townsfolk))
                {
                    characters.Add(townsfolk);
                }
            }

            return await GetCharacterSeenByRavenkeeperFromList(ravenkeeper, target, characters);
        }

        private async Task<Character> GetCharacterSeenByRavenkeeperFromList(Player ravenkeeper, Player target, IReadOnlyCollection<Character> characters)
        {
            if (characters.Count == 1)
            {
                return characters.First();
            }

            return await storyteller.GetCharacterForRavenkeeper(ravenkeeper, target, characters);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
    }
}
