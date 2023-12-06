using Clocktower.Agent;

namespace Clocktower.Game
{
    /// <summary>
    /// Represents a player in a Blood on the Clocktower game and all information about that player.
    /// A player has an agent that actually takes actions for it.
    /// </summary>
    public class Player
    {
        public string Name { get; }
        public IAgent Agent { get; }

        public bool Alive => alive && !Tokens.Contains(Token.DiedAtNight);
        public bool HasGhostVote { get; private set; } = true;

        public Character? RealCharacter { get; private set; }
        public Alignment? RealAlignment { get; private set; }

        public Character? Character => believedCharacter ?? RealCharacter;
        public Alignment? Alignment => believedAlignment ?? RealAlignment;

        public bool DrunkOrPoisoned => RealCharacter.HasValue && RealCharacter.Value == Game.Character.Drunk;

        public List<Token> Tokens { get; } = new();

        public Player(string name, IAgent agent)
        {
            Name = name;
            Agent = agent;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            RealCharacter = character;
            RealAlignment = alignment;

            believedCharacter = null;
            believedAlignment = null;

            Agent.AssignCharacter(character, alignment);
        }

        public void AssignCharacter(Character character, Alignment alignment,
                                    Character believedCharacter, Alignment believedAlignment)
        {
            RealCharacter = character;
            RealAlignment = alignment;

            this.believedCharacter = believedCharacter;
            this.believedAlignment = believedAlignment;

            Agent.AssignCharacter(believedCharacter, believedAlignment);
        }

        public void Kill()
        {
            alive = false;
            Agent.YouAreDead();
        }

        public void UseGhostVote()
        {
            HasGhostVote = false;
        }

        private Character? believedCharacter;
        private Alignment? believedAlignment;

        private bool alive = true;
    }
}
