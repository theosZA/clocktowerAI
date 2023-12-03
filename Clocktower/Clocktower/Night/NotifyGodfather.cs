﻿using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Night
{
    internal class NotifyGodfather : INightEvent
    {
        public NotifyGodfather(IStoryteller storyteller, Grimoire grimoire)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
        }

        public void RunEvent(Action onEventFinished)
        {
            var godfather = grimoire.GetPlayer(Character.Godfather);
            if (godfather != null)
            {
                var outsiders = grimoire.GetOutsiders().ToList();
                godfather.Agent.NotifyGodfather(outsiders);
                storyteller.NotifyGodfather(godfather, outsiders);
            }

            onEventFinished();
        }

        private IStoryteller storyteller;
        private Grimoire grimoire;
    }
}
