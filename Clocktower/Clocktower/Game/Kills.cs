using Clocktower.Storyteller;

namespace Clocktower.Game
{
    internal class Kills
    {
        public Kills(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public async Task Execute(Player player)
        {
            player.Tokens.Add(Token.Executed, player);
            await DayKill(player, killer: null);

            if (player.Character == Character.Saint && !player.DrunkOrPoisoned)
            {
                grimoire.EndGame(player.Alignment == Alignment.Good ? Alignment.Evil : Alignment.Good);
            }
        }

        public async Task DayKill(Player player, Player? killer)
        {
            await HandleDayDeath(player, killer);
            player.Kill();
        }

        public async Task NightKill(Player player, Player? killer)
        {
            if (player.Character == Character.Mayor && !player.DrunkOrPoisoned && player.Alive && killer?.Character != Character.Assassin)
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

        private async Task HandleDayDeath(Player dyingPlayer, Player? killer)
        {
            grimoire.RemoveTokensForCharacter(dyingPlayer.RealCharacter);
            ProcessDayTriggersForOtherCharacters(dyingPlayer);
            await ProcessTriggersForOtherCharacters(dyingPlayer, killer);
        }

        private async Task HandleNightDeath(Player dyingPlayer, Player? killer)
        {
            grimoire.RemoveTokensForCharacter(dyingPlayer.RealCharacter);
            await ProcessTriggersForOtherCharacters(dyingPlayer, killer);
        }

        private void ProcessDayTriggersForOtherCharacters(Player dyingPlayer)
        {
            // Godfather
            if (dyingPlayer.CharacterType == CharacterType.Outsider)
            {
                foreach (var godfather in grimoire.GetLivingPlayers(Character.Godfather))
                {
                    godfather.Tokens.Add(Token.GodfatherKillsTonight, dyingPlayer);
                }
            }
        }

        private async Task ProcessTriggersForOtherCharacters(Player dyingPlayer, Player? killer)
        {
            // Scarlet Woman
            bool scarletWomanTriggered = false;
            if (dyingPlayer.CharacterType == CharacterType.Demon && grimoire.Players.Count(player => player.Alive) >= 5)
            {
                var scarletWoman = grimoire.GetLivingPlayers(Character.Scarlet_Woman).FirstOrDefault(player => player.Alive && !player.DrunkOrPoisoned); // shouldn't be more than 1 Scarlet Woman
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

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
