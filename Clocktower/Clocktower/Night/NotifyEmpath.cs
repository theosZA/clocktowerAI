using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class NotifyEmpath : INightEvent
    {
        public NotifyEmpath(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var empath = grimoire.GetAlivePlayer(Character.Empath);
            if (empath != null)
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

                empath.Agent.NotifyEmpath(livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
                storyteller.NotifyEmpath(empath, livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
            }

            onEventFinished();
        }

        private static Alignment GetEmpathAlignment(Player player)
        {
            if (player.RealCharacter.HasValue && player.RealCharacter == Character.Recluse)
            {
                return Alignment.Evil;  // Note that this doesn't have to be evil, but then requires storyteller input.
            }
            return player.RealAlignment ?? Alignment.Good;
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
