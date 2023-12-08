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

        public bool Alive => alive && !Tokens.Contains(Token.DiedAtNight) && !Tokens.Contains(Token.KilledByDemon);
        public bool HasGhostVote { get; private set; } = true;

        /// <summary>
        /// The character whose ability the player has. This corresponds to the character token that would be in the physical grimoire.
        /// In some cases the player's real character is something else (e.g. Drunk, Lunatic), and in other cases the player just has
        /// this character's ability (e.g. Philosopher, Alchemist). In these situations a Token is added to Tokens that indicates the
        /// real situation.
        /// </summary>
        public Character Character { get; private set; }
        /// <summary>
        /// The player's real alignment. The player may believe differently, e.g. if they're a Marionette or Lunatic.
        /// </summary>
        public Alignment Alignment { get; private set; }

        public CharacterType CharacterType
        {
            get => (int)Character switch
            {
                < 1000 => Tokens.Contains(Token.IsTheDrunk) ? CharacterType.Outsider : CharacterType.Townsfolk,
                < 2000 => CharacterType.Outsider,
                < 3000 => CharacterType.Minion,
                _ => CharacterType.Demon
            };
        }

        public bool DrunkOrPoisoned => Tokens.Where(token => token == Token.IsTheDrunk || token == Token.SweetheartDrunk || token == Token.PoisonedByPoisoner).Any();

        public List<Token> Tokens { get; } = new();

        public bool CanRegisterAsEvil => Alignment == Alignment.Evil || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsDemon => CharacterType == CharacterType.Demon || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsMinion => CharacterType == CharacterType.Minion || (Character == Character.Recluse && !DrunkOrPoisoned);

        public bool CanBeKilledByDemon => !((Character == Character.Soldier && !DrunkOrPoisoned) || Tokens.Contains(Token.ProtectedByMonk));

        public Player(string name, IAgent agent, Character character, Alignment alignment)
        {
            Name = name;
            Agent = agent;
            Character = character;
            Alignment = alignment;
        }

        public void ChangeCharacter(Character newCharacter)
        {
            Character = newCharacter;
            Agent.AssignCharacter(Character, Alignment);
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

        private bool alive = true;
    }
}
