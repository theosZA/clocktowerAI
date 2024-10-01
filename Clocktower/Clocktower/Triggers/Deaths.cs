using Clocktower.Game;
using Clocktower.Storyteller;

namespace Clocktower.Triggers
{
    internal class Deaths
    {
        public Deaths(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> scriptCharacters)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;

            deathTriggers = BuildDeathTriggers(scriptCharacters).ToList();
        }

        public async Task Execute(Player player)
        {
            player.Tokens.Add(Token.Executed, player);
            grimoire.MostRecentlyExecutedPlayerToDie = player;
            await DayKill(player, execution: true);
        }

        public async Task DayKill(Player player, Player? killer = null, bool execution = false)
        {
            var deathInformation = new DeathInformation
            {
                dyingPlayer = player,
                killingPlayer = killer,
                duringDay = true,
                executed = execution
            };
            await HandleDeath(deathInformation);

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

            if (!player.Alive)
            {
                return;
            }

            var deathInformation = new DeathInformation
            {
                dyingPlayer = player,
                killingPlayer = killer,
                duringDay = false
            };
            await HandleDeath(deathInformation);

            player.Tokens.Add(Token.DiedAtNight, killer ?? player);
        }

        private async Task HandleDeath(DeathInformation deathInformation)
        {
            foreach (var deathTrigger in deathTriggers)
            {
                await deathTrigger.RunTrigger(deathInformation);
            }
            if (deathInformation.dyingPlayer.CharacterType != CharacterType.Minion || !deathInformation.dyingPlayer.Tokens.HasToken(Token.MinionKilledByVigormortis))
            {
                grimoire.ClearTokensOnPlayerDeath(deathInformation.dyingPlayer);
            }
        }

        private IEnumerable<IDeathTrigger> BuildDeathTriggers(IReadOnlyCollection<Character> scriptCharacters)
        {
            if (scriptCharacters.Contains(Character.Saint))
            {
                yield return new SaintDeathTrigger(grimoire);
            }
            if (scriptCharacters.Contains(Character.Godfather))
            {
                yield return new GodfatherDeathTrigger(grimoire);
            }
            if (scriptCharacters.Contains(Character.Cannibal))
            {
                yield return new CannibalDeathTrigger(storyteller, grimoire, scriptCharacters);
            }
            if (scriptCharacters.Contains(Character.Ravenkeeper))
            {
                yield return new RavenkeeperDeathTrigger(storyteller, grimoire, scriptCharacters);
            }
            if (scriptCharacters.Contains(Character.Scarlet_Woman))
            {
                yield return new ScarletWomanDeathTrigger(storyteller, grimoire);
            }
            if (scriptCharacters.Contains(Character.Imp))
            {
                yield return new ImpStarPassDeathTrigger(storyteller, grimoire);
            }
            if (scriptCharacters.Contains(Character.Sweetheart))
            {
                yield return new SweetheartDeathTrigger(storyteller, grimoire);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly List<IDeathTrigger> deathTriggers;
    }
}
