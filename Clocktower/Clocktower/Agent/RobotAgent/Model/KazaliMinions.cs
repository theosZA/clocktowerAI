using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Selection;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying which players are to be which minions.
    /// </summary>
    internal class KazaliMinions
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public IEnumerable<PlayerAsCharacter> MinionAssignments { get; set; } = Enumerable.Empty<PlayerAsCharacter>();

        public string ErrorText(KazaliMinionsSelection kazaliMinionsSelection)
        {
            var minionAssignments = MinionAssignments.ToList();
            if (minionAssignments.Count != kazaliMinionsSelection.MinionCount)
            {
                return $"You must have exactly {kazaliMinionsSelection.MinionCount} minion assignments.";
            }

            var players = new HashSet<Player>();
            var characters = new HashSet<Character>();
            foreach (var minionAssignment in minionAssignments)
            {
                var player = TextParser.ReadPlayerFromText(minionAssignment.Player, kazaliMinionsSelection.PossiblePlayers);
                if (player == null)
                {
                    return $"{minionAssignment.Player} is not a valid choice of player. Choose from: {string.Join(", ", kazaliMinionsSelection.PossiblePlayers.Select(player => player.Name))}.";
                }
                if (players.Contains(player))
                {
                    return $"{minionAssignment.Player} was picked more than once. Each player may only be chosen once.";
                }
                players.Add(player);

                var character = TextParser.ReadCharacterFromText(minionAssignment.Character, kazaliMinionsSelection.MinionCharacters);
                if (character == null)
                {
                    return $"{minionAssignment.Character} is not a valid choice of Minion character. Choose from: {string.Join(", ", kazaliMinionsSelection.MinionCharacters.Select(character => TextUtilities.CharacterToText(character)))}.";
                }
                if (characters.Contains(character.Value))
                {
                    return $"{minionAssignment.Character} was picked more than once. Each Minion character may only be chosen once.";
                }
                characters.Add(character.Value);
            }

            return "You must provide a valid assignment of players to Minion characters.";
        }
    }
}
