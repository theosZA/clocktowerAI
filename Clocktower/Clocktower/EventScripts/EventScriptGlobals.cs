using Clocktower.Game;

namespace Clocktower.EventScripts
{
    public class EventScriptGlobals
    {
        public IGameSetup Setup { get; }
        public Grimoire Grimoire { get; }

        public EventScriptGlobals(IGameSetup setup, Grimoire grimoire)
        {
            Setup = setup;
            Grimoire = grimoire;
        }
    }
}
