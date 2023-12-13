using Clocktower.Game;

namespace Clocktower.Events
{
    internal static class GameEventExtensions
    {
        public static async Task RunEvents(this IEnumerable<IGameEvent> gameEvents, Grimoire grimoire)
        {
            foreach (var gameEvent in gameEvents)
            {
                await gameEvent.RunEvent();
                if (grimoire.Finished)
                {
                    return;
                }
            }
        }
    }
}
