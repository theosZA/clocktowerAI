using Clocktower.Agent;

namespace Clocktower.Game
{
    /// <summary>
    /// Represents a player in a Blood on the Clocktower game and all information about that player.
    /// A player has an agent that actually takes actions for it.
    /// </summary>
    public class Player
    {
        public string Name => Agent.PlayerName;
        public IAgent Agent { get; }

        public bool Alive => alive && !Tokens.HasToken(Token.DiedAtNight);
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

        /// <summary>
        /// The player's real character. The player may believe differently, e.g. if they're a Drunk or Lunatic.
        /// </summary>
        public Character RealCharacter => Tokens.Character ?? Character;
        /// <summary>
        /// The player's real character type. The player may believe differently, e.g. if they're a Drunk or Lunatic.
        /// </summary>
        public CharacterType CharacterType => RealCharacter.CharacterType();

        public bool DrunkOrPoisoned => Tokens.DrunkOrPoisoned;

        public TokensOnPlayer Tokens { get; }

        public bool CanRegisterAsGood => Alignment == Alignment.Good || (Character == Character.Spy && !DrunkOrPoisoned);
        public bool CanRegisterAsTownsfolk => CharacterType == CharacterType.Townsfolk || (Character == Character.Spy && !DrunkOrPoisoned);
        public bool CanRegisterAsOutsider => CharacterType == CharacterType.Outsider || (Character == Character.Spy && !DrunkOrPoisoned);

        public bool CanRegisterAsEvil => Alignment == Alignment.Evil || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsMinion => CharacterType == CharacterType.Minion || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsDemon => CharacterType == CharacterType.Demon || (Character == Character.Recluse && !DrunkOrPoisoned);

        public bool ProtectedFromDemonKill => (Character == Character.Soldier && !DrunkOrPoisoned) || Tokens.HasHealthyToken(Token.ProtectedByMonk);

        public Player(IAgent agent, Character character, Alignment alignment)
        {
            Agent = agent;
            Character = character;
            Alignment = alignment;
            Tokens = new(this);
        }

        public void ChangeCharacter(Character newCharacter)
        {
            Character = newCharacter;
            Agent.AssignCharacter(Character, Alignment);
        }

        public void GainCharacterAbility(Character newCharacter)
        {
            if (Character == Character.Philosopher)
            {
                Tokens.Add(Token.IsThePhilosopher, this);
            }

            Character = newCharacter;
            Agent.GainCharacterAbility(newCharacter);
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
