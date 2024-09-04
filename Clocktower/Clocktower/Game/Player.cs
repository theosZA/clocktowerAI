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
        public Alignment Alignment { get; set; }

        /// <summary>
        /// The player's real character. The player may believe differently, e.g. if they're a Drunk or Lunatic.
        /// </summary>
        public Character RealCharacter => Tokens.Character ?? Character;
        /// <summary>
        /// The player's real character type. The player may believe differently, e.g. if they're a Drunk or Lunatic.
        /// </summary>
        public CharacterType CharacterType => RealCharacter.CharacterType();

        /// <summary>
        /// All characters a player has previously been ending with their current character, e.g. if they were a Scarlet Woman who became the Imp.
        /// In cases where additional characters are applicable (e.g. Drunk or Philosopher), they are included in a list.
        /// </summary>
        public IReadOnlyCollection<List<Character>> CharacterHistory => characterHistory;

        /// <summary>
        /// Returns true if the player doesn't have the ability they think they do. Technically this includes a few cases where they aren't actually
        /// drunk or poisoned like for a Marionette or Lunatic.
        /// </summary>
        public bool DrunkOrPoisoned => Tokens.DrunkOrPoisoned;

        public TokensOnPlayer Tokens { get; }

        public bool CanRegisterAsGood => Alignment == Alignment.Good || (Character == Character.Spy && !DrunkOrPoisoned);
        public bool CanRegisterAsTownsfolk => CharacterType == CharacterType.Townsfolk || (Character == Character.Spy && !DrunkOrPoisoned);
        public bool CanRegisterAsOutsider => CharacterType == CharacterType.Outsider || (Character == Character.Spy && !DrunkOrPoisoned);

        public bool CanRegisterAsEvil => Alignment == Alignment.Evil || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsMinion => CharacterType == CharacterType.Minion || (Character == Character.Recluse && !DrunkOrPoisoned);
        public bool CanRegisterAsDemon => CharacterType == CharacterType.Demon || (Character == Character.Recluse && !DrunkOrPoisoned);

        public bool ProtectedFromDemonKill => (Character == Character.Soldier && !DrunkOrPoisoned) || Tokens.HasHealthyToken(Token.ProtectedByMonk);

        public Character? CannibalAbility { get; set; }

        public Player(Grimoire grimoire, IAgent agent, Character character, Alignment alignment)
        {
            Agent = agent;
            Character = character;
            Alignment = alignment;
            Tokens = new(grimoire, this);

            if (character == Character.Juggler)
            {
                Tokens.Add(Token.JugglerBeforeFirstDay, this);
            }
        }

        public async Task ChangeCharacter(Character newCharacter)
        {
            var currentCharacterInfo = new List<Character>();
            if (Tokens.HasToken(Token.IsTheDrunk))
            {
                currentCharacterInfo.Add(Character.Drunk);
            }
            if (Tokens.HasToken(Token.IsThePhilosopher))
            {
                currentCharacterInfo.Add(Character.Philosopher);
            }
            currentCharacterInfo.Add(Character);
            characterHistory.Add(currentCharacterInfo);

            Character = newCharacter;
            Alignment = newCharacter.Alignment();
            await Agent.AssignCharacter(Character, Alignment);

            if (newCharacter == Character.Juggler)
            {
                Tokens.Add(Token.JugglerBeforeFirstDay, this);
            }
        }

        public async Task GainCharacterAbility(Character newCharacter)
        {
            if (CannibalAbility == Character.Philosopher)
            {
                Tokens.Add(Token.IsTheCannibalPhilosopher, this);
            }
            else if (ShouldRunAbility(Character.Philosopher))
            {
                Tokens.Add(Token.IsThePhilosopher, this);
            }

            if (ShouldRunAbility(Character.Cannibal) && ShouldRunAbility(Character.Philosopher))
            {
                CannibalAbility = newCharacter;
            }
            else
            {
                Character = newCharacter;
            }
            await Agent.OnGainCharacterAbility(newCharacter);

            if (newCharacter == Character.Juggler)
            {
                Tokens.Add(Token.JugglerBeforeFirstDay, this);
            }
        }

        public bool CanRegisterAs(Character character)
        {
            if (RealCharacter == character)
            {
                return true;
            }

            if (CharacterType == character.CharacterType())
            {
                return false;
            }

            return character.CharacterType() switch
            {
                CharacterType.Demon => CanRegisterAsDemon,
                CharacterType.Minion => CanRegisterAsMinion,
                CharacterType.Outsider => CanRegisterAsOutsider,
                CharacterType.Townsfolk => CanRegisterAsTownsfolk,
                _ => false,
            };
        }

        /// <summary>
        /// Does this player really have the specified ability? This will only be if they are neither
        /// drunk nor poisoned, alive and have not already used it in the case of once-per-game abilities.
        /// </summary>
        /// <param name="characterAbility">Character ability to check for.</param>
        /// <returns>True if the player really has the specified ability.</returns>
        public bool HasHealthyAbility(Character characterAbility)
        {
            if (!ShouldRunAbility(characterAbility))
            {
                return false;
            }

            if (DrunkOrPoisoned)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Should we treat the player like they have the specified ability? This will include not
        /// only cases where they actually do have the ability, but also cases where they only
        /// think that they do, like if they are poisoned or drunk.
        /// </summary>
        /// <param name="characterAbility">Character ability to check for.</param>
        /// <returns>True if we should treat the player like they have the ability, whether they really have the ability or not.</returns>
        public bool ShouldRunAbility(Character characterAbility)
        {
            if (!(Character == characterAbility || (Character == Character.Cannibal && CannibalAbility == characterAbility)))
            {
                return false;
            }

            if (!Alive)
            {
                return false;
            }

            if (Tokens.HasToken(Token.IsTheBadPhilosopher) && !DrunkOrPoisoned)
            {   // The philosopher was drunk or poisoned when choosing their ability.
                // The storyteller pretends that they have the ability that they believed that they gained.                 
                // But they are no longer drunk or poisoned, so we don't pretend they have the ability.
                return false;
            }

            // If the ability is a once-per-game ability, then we should only run it if they have not yet used it.
            switch (characterAbility)
            {
                case Character.Fisherman:
                case Character.Slayer:
                case Character.Virgin:
                case Character.Nightwatchman:
                case Character.Assassin:
                    if (Tokens.HasToken(Token.UsedOncePerGameAbility))
                    {
                        return false;
                    }
                    break;

                case Character.Juggler:
                    if (!Tokens.HasToken(Token.JugglerFirstDay))
                    {
                        return false;
                    }
                    break;
            }

            return true;
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

        private readonly List<List<Character>> characterHistory = new();
    }
}
