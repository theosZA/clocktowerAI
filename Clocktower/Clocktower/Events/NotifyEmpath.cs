using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Events
{
    internal class NotifyEmpath : IGameEvent
    {
        public NotifyEmpath(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public Task RunEvent()
        {
            foreach (var empath in grimoire.GetLivingPlayers(Character.Empath))
            {
                var livingNeighbours = grimoire.GetLivingNeighbours(empath);

                int evilCount = 0;
                if (GetEmpathAlignment(livingNeighbours.Item1) == Alignment.Evil)
                {
                    ++evilCount;
                }
                if (GetEmpathAlignment(livingNeighbours.Item2) == Alignment.Evil)
                {
                    ++evilCount;
                }

                if (empath.DrunkOrPoisoned)
                {
                    evilCount = (evilCount == 0 ? 1 : 0);
                }

                empath.Agent.NotifyEmpath(livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
                storyteller.NotifyEmpath(empath, livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
            }

            return Task.CompletedTask;
        }

        private static Alignment GetEmpathAlignment(Player player)
        {
            if (player.Character == Character.Recluse)
            {
                return Alignment.Evil;  // Note that this doesn't have to be evil, but then requires storyteller input.
            }
            return player.Alignment;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
    }
}
