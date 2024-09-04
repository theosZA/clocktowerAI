using Clocktower.Game;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying a player and their character.
    /// </summary>
    internal class PlayerAsCharacter
    {
        [Required(AllowEmptyStrings = true)]
        public string Player { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        public string Character { get; set; } = string.Empty;

        public (Player, Character)? GetAssignment(IReadOnlyCollection<Player> players, IReadOnlyCollection<Character> characters)
        {
            var player = TextParser.ReadPlayerFromText(Player, players);
            if (player == null)
            {
                return null;
            }

            var character = TextParser.ReadCharacterFromText(Character, characters);
            if (character == null)
            {
                return null;
            }

            return (player, character.Value);
        }
    }
}
