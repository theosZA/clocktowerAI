using Clocktower.Game;
using Clocktower.Setup;

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
