using Clocktower.Game;
using Clocktower.Options;
using System.ComponentModel.DataAnnotations;

namespace Clocktower.Agent.RobotAgent.Model
{
    /// <summary>
    /// Robot agent model for specifying which players are to be which minions.
    /// </summary>
    internal class KazaliMinions : IOptionSelection
    {
        [Required(AllowEmptyStrings = true)]
        public string Reasoning { get; set; } = string.Empty;

        [Required]
        public IEnumerable<PlayerAsCharacter> MinionAssignments { get; set; } = Enumerable.Empty<PlayerAsCharacter>();

        public IOption? PickOption(IReadOnlyCollection<IOption> options)
        {
            var kazaliMinionsOption = (KazaliMinionsOption)options.First(option => option is KazaliMinionsOption);
            var minionAssignments = MinionAssignments.Select(assignment => assignment.GetAssignment(kazaliMinionsOption.PossiblePlayers, kazaliMinionsOption.MinionCharacters)).ToList();
            if (minionAssignments.Count != kazaliMinionsOption.MinionCount)
            {
                return null;
            }
            if (minionAssignments.Any(assignment => !assignment.HasValue))
            {
                return null;
            }

            kazaliMinionsOption.ChooseMinions(minionAssignments.Select(assignment => assignment!.Value));
            if (!kazaliMinionsOption.AssignmentOk)
            {
                return null;
            }

            return kazaliMinionsOption;
        }

        public string NoMatchingOptionPrompt(IReadOnlyCollection<IOption> options)
        {
            var kazaliMinionsOption = (KazaliMinionsOption)options.First(option => option is KazaliMinionsOption);
            var minionAssignments = MinionAssignments.ToList();
            if (minionAssignments.Count != kazaliMinionsOption.MinionCount)
            {
                return $"You must have exactly {kazaliMinionsOption.MinionCount} minion assignments.";
            }

            var players = new HashSet<Player>();
            var characters = new HashSet<Character>();
            foreach (var minionAssignment in minionAssignments)
            {
                var player = TextParser.ReadPlayerFromText(minionAssignment.Player, kazaliMinionsOption.PossiblePlayers);
                if (player == null)
                {
                    return $"{minionAssignment.Player} is not a valid choice of player. Choose from: {string.Join(", ", kazaliMinionsOption.PossiblePlayers.Select(player => player.Name))}.";
                }
                if (players.Contains(player))
                {
                    return $"{minionAssignment.Player} was picked more than once. Each player may only be chosen once.";
                }
                players.Add(player);

                var character = TextParser.ReadCharacterFromText(minionAssignment.Character, kazaliMinionsOption.MinionCharacters);
                if (character == null)
                {
                    return $"{minionAssignment.Character} is not a valid choice of Minion character. Choose from: {string.Join(", ", kazaliMinionsOption.MinionCharacters.Select(character => TextUtilities.CharacterToText(character)))}.";
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
