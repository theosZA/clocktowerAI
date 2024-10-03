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

        /// <summary>
        /// Process a player dying at night, usually at the hands of another player. The kill may be
        /// blocked or redirected depending on character abilities.
        /// </summary>
        /// <param name="player">The player due to die.</param>
        /// <param name="killer">The player, if any, instigating the kill.</param>
        /// <param name="shouldContinueWithKillOnRedirect">
        /// An optional async predicate in the case that the kill is redirected. Should return false if the redirected kill should immediately fail.
        /// If the predicate is not provided, it is assumed to continue with the kill as if it was the original target.
        /// </param>
        /// <returns>
        /// The player, if any, that actually died.
        /// This will be null if the kill failed for any reason including if the player was already dead.
        /// And this may not be the same as the provided player if the kill was redirected.
        /// </returns>
        public async Task<Player?> NightKill(Player player, Player? killer, Func<Player, Task<bool>>? shouldContinueWithKillOnRedirect = null)
        {
            if (player.HasHealthyAbility(Character.Mayor)
                && !(killer?.CharacterType == CharacterType.Demon && player.ProtectedFromDemonKill)
                && killer?.Character != Character.Assassin)
            {
                player = await storyteller.GetMayorBounce(player, killer, grimoire.Players);
                if (shouldContinueWithKillOnRedirect != null && !await shouldContinueWithKillOnRedirect(player))
                {
                    return null;
                }
            }

            if (grimoire.Players.Any(player => player.Tokens.HasHealthyToken(Token.KilledByLycanthrope))
                && killer?.Character != Character.Assassin)
            {
                return null;
            }

            if (killer?.CharacterType == CharacterType.Demon && player.ProtectedFromDemonKill)
            {
                return null;
            }

            if (!player.Alive)
            {
                return null;
            }

            var deathInformation = new DeathInformation
            {
                dyingPlayer = player,
                killingPlayer = killer,
                duringDay = false
            };
            await HandleDeath(deathInformation);

            player.Tokens.Add(Token.DiedAtNight, killer ?? player);

            return player;
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
