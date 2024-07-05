namespace Clocktower.Game
{
    public interface IGameSetup
    {
        /// <summary>
        /// The name of the script defining the characters available in this game.
        /// </summary>
        string ScriptName { get; }

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
        /// Checks if the given character is to be included in the game even if that character hasn't been assigned to a seat yet.
        /// </summary>
        /// <param name="character">Character to check if it is to be included in the game.</param>
        /// <returns>True if the given character is to be included in the game.</returns>
        bool IsCharacterSelected(Character character);
    }
}
