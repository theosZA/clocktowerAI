using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Options
{
    /// <summary>
    /// Represents claiming Minion and guessing a particular player as the Damsel.
    /// </summary>
    internal class MinionGuessingDamselOption : IOption
    {
        public string Name => $"Minion guessing Damsel...";

        public Player? Target { get; private set; }

        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }

        public MinionGuessingDamselOption(IReadOnlyCollection<Player> players)
        {
            PossiblePlayers = players;
        }
        
        public void SetTarget(Player target)
        {
            if (PossiblePlayers.Contains(target))
            {
                Target = target;
            }
        }

        public void SetTargetFromText(string text)
        {
            var target = TextParser.ReadPlayerFromText(text, PossiblePlayers);
            if (target != null)
            {
                Target = target;
            }
        }
    }
}
