using Clocktower.Events;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    public interface IGameSetup
    {
        /// <summary>
        /// The characters available in this game.
        /// </summary>
        IReadOnlyCollection<Character> Script { get; }

        /// <summary>
        /// The number of players in the game.
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// The characters assigned to each seat (0...n-1).
        /// </summary>
        Character[] Characters { get; }

        /// <summary>
        /// Additional events that have to be run to complete setup.
        /// These are typically events that will require storyteller intervention, e.g. assigning the Drunk.
        /// </summary>
        IEnumerable<IGameEvent> BuildAdditionalSetupEvents(IStoryteller storyteller, Grimoire grimoire);
    }
}
