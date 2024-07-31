using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class EndDay : IGameEvent
    {
        public EndDay(IStoryteller storyteller, Grimoire grimoire, Kills kills, IGameObserver observers)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.kills = kills;
            this.observers = observers;
        }

        public async Task RunEvent()
        {
            if (grimoire.PlayerToBeExecuted == null)
            {
                await observers.DayEndsWithNoExecution();
                CheckForMayorWin();
            }
            else
            {
                bool playerDies = await DoesExecutedPlayerDie(grimoire.PlayerToBeExecuted);
                await observers.PlayerIsExecuted(grimoire.PlayerToBeExecuted, playerDies);
                if (playerDies)
                {
                    await kills.Execute(grimoire.PlayerToBeExecuted);
                }
                grimoire.PlayerToBeExecuted = null;
            }

            if (!grimoire.Finished)
            {
                await observers.AnnounceLivingPlayers(grimoire.Players);
            }
        }

        private async Task<bool> DoesExecutedPlayerDie(Player executedPlayer)
        {
            if (!executedPlayer.Alive)
            {
                return false;
            }

            if (executedPlayer.Tokens.HasHealthyToken(Token.ProtectedByDevilsAdvocate))
            {
                if (executedPlayer.HasHealthyAbility(Character.Tinker))
                {
                    return await storyteller.ShouldKillTinker(executedPlayer);
                }

                return false;
            }

            if (executedPlayer.CanRegisterAsGood && grimoire.PlayersWithHealthyAbility(Character.Pacifist).Any())
            {
                return !await storyteller.ShouldSaveWithPacifist(grimoire.PlayersWithHealthyAbility(Character.Pacifist).First(), executedPlayer);
            }

            return true;
        }

        private void CheckForMayorWin()
        {
            if (grimoire.Players.Alive() != 3)
            {
                return;
            }
            var mayors = grimoire.PlayersWithHealthyAbility(Character.Mayor).ToList();
            // There really shouldn't be more than one player with a Mayor ability here, but just in case
            // we apply the rule that any good mayor win trumps any evil mayor win.
            if (mayors.Any(mayor => mayor.Alignment == Alignment.Good))
            {
                grimoire.EndGame(Alignment.Good);
            }
            else if (mayors.Any(mayor => mayor.Alignment == Alignment.Evil))
            {
                grimoire.EndGame(Alignment.Evil);
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Kills kills;
        private readonly IGameObserver observers;
    }
}
