using Clocktower.Agent;
using Clocktower.Game;

namespace Clocktower.Options
{
    /// <summary>
    /// Represents claiming Slayer and shooting a player. Of course this will have no effect if the player is not
    /// the Slayer, but it needs to be available to everyone so that they can bluff the character.
    /// </summary>
    internal class SlayerShotOption : IOption
    {
        public string Name => $"Slayer...";

        public Player? Target { get; private set; }

        public IReadOnlyCollection<Player> PossiblePlayers { get; private init; }

        public SlayerShotOption(IReadOnlyCollection<Player> players)
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
