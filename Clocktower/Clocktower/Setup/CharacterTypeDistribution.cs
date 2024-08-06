using Clocktower.Game;
using System.ComponentModel;

namespace Clocktower.Setup
{
    /// <summary>
    /// Handles the distribution of character types going into the bag.
    /// </summary>
    internal class CharacterTypeDistribution
    {
        public CharacterTypeDistribution(Random random, IReadOnlyCollection<Character> script, Func<Character, bool> isCharacterSelected)
        {
            this.random = random;
            this.script = script;
            this.isCharacterSelected = isCharacterSelected;
        }

        /// <summary>
        /// Returns all possible values for the number of the given character type that can go in the bag.
        /// This takes into account the currently selected characters, but makes no assumptions on which Outsider modifications
        /// are made when there is more than one possibility.
        /// </summary>
        /// <param name="characterType">The character type whose possible counts are returned.</param>
        /// <param name="playerCount">The number of players in the game.</param>
        /// <returns>A collection of all possible counts of the given character type. No duplicates will be included.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if an invalid character type is provided.</exception>
        public IEnumerable<int> GetPossibleCounts(CharacterType characterType, int playerCount)
        {
            return characterType switch
            {
                CharacterType.Townsfolk => GetRequiredTownsfolk(playerCount),
                CharacterType.Outsider => GetRequiredOutsiders(playerCount, Array.Empty<Character>()),
                CharacterType.Minion => new[] { GetRequiredMinions(playerCount) },
                CharacterType.Demon => new[] { 1 },
                _ => throw new InvalidEnumArgumentException(nameof(characterType))
            };
        }

        /// <summary>
        /// Returns all possible values for the number of Outsiders assuming the given character was excluded
        /// (i.e. they could not modify the Outsider count).
        /// </summary>
        /// <param name="playerCount">The number of players in the game.</param>
        /// <param name="characterToExclude">Character to exclude from calculating the required Outsider count.</param>
        /// <returns>A collection of possible Outsider counts.</returns>
        public IEnumerable<int> GetPossibleOutsiderCountsExcludingCharacter(int playerCount, Character characterToExclude)
        {
            return GetRequiredOutsiders(playerCount, new[] { characterToExclude });
        }

        /// <summary>
        /// Returns the calculated modification to the Outsider count due to the inclusion of Demon and Minion characters.
        /// Where multiple possible modification values are possible for a character, one value is randomly chosen.
        /// </summary>
        /// <returns>The modification to the Outsider count.</returns>
        public int GetRandomOutsiderModificationByEvil()
        {
            int outsiderModification = 0;

            if (isCharacterSelected(Character.Baron))
            {
                outsiderModification += 2;
            }
            if (isCharacterSelected(Character.Godfather))
            {
                if (random.Next(10) == 0)
                {
                    outsiderModification--;
                }
                else
                {
                    outsiderModification++;
                }
            }

            return outsiderModification;
        }

        /// <summary>
        /// Returns the calculated modification to the Outsider count due to the inclusion of Townsfolk characters.
        /// Where multiple possible modification values are possible for a character, one value is randomly chosen.
        /// </summary>
        /// <returns>The modification to the Outsider count.</returns>
        public int GetRandomOutsiderModificationByGood()
        {
            int outsiderModification = 0;

            if (isCharacterSelected(Character.Balloonist) && random.Next(2) == 0)
            {
                outsiderModification++;
            }

            return outsiderModification;
        }

        /// <summary>
        /// Calculates how many Townsfolk should be included.
        /// </summary>
        /// <param name="playerCount">The number of players in the game.</param>
        /// <param name="outsiderModification">The modification to the outsider count due to non-Townsfolk characters.</param>
        /// <returns>The number of Townsfolk that should be included.</returns>
        public int GetTownsfolkCount(int playerCount, int outsiderModification)
        {
            int demonCount = 1;
            int minionCount = GetRequiredMinions(playerCount);
            int outsiderCount = GetOutsiderCount(playerCount, outsiderModification);
            int townsfolkCount = playerCount - (outsiderCount + minionCount + demonCount);

            if (isCharacterSelected(Character.Marionette))
            {
                townsfolkCount++;
            }
            if (isCharacterSelected(Character.Drunk))
            {
                townsfolkCount++;
            }

            return townsfolkCount;
        }

        /// <summary>
        /// Calculates how many Outsiders should be included.
        /// </summary>
        /// <param name="playerCount">The number of players in the game.</param>
        /// <param name="outsiderModification">The modification to the outsider count.</param>
        /// <returns>The number of Outsiders that should be included.</returns>
        public int GetOutsiderCount(int playerCount, int outsiderModification)
        {
            return Clamp(GetBaseOutsiderCount(playerCount) + outsiderModification, 0, GetMaximumOutsiderCount());
        }

        /// <summary>
        /// Calculates how many Minions should be included.
        /// </summary>
        /// <param name="playerCount">The number of players in the game.</param>
        /// <returns>The number of Minions that should be included.</returns>
        public int GetMinionCount(int playerCount)
        {
            return GetRequiredMinions(playerCount);
        }

        private IEnumerable<int> GetRequiredTownsfolk(int playerCount)
        {
            int demonCount = 1;
            int minionCount = GetRequiredMinions(playerCount);

            foreach (int outsiderCount in GetRequiredOutsiders(playerCount, Array.Empty<Character>()))
            {
                int townsfolkCount = playerCount - (outsiderCount + minionCount + demonCount);

                if (isCharacterSelected(Character.Drunk))
                {
                    ++townsfolkCount;
                }
                if (isCharacterSelected(Character.Marionette))
                {
                    ++townsfolkCount;
                }

                yield return townsfolkCount;
            }
        }

        private IEnumerable<int> GetRequiredOutsiders(int playerCount, IEnumerable<Character> excludingCharacters)
        {
            int baseOutsiders = playerCount switch
            {
                7 => 0,
                8 => 1,
                9 => 2,
                10 => 0,
                11 => 1,
                12 => 2,
                13 => 0,
                14 => 1,
                _ => 2
            };

            var possibleOutsiderAdjustments = new List<IReadOnlyCollection<int>>();
            var outsiderAdjustingCharacters = new[] { Character.Baron, Character.Godfather, Character.Balloonist }.Except(excludingCharacters).ToList();
            foreach (var character in outsiderAdjustingCharacters)
            {
                if (isCharacterSelected(character))
                {
                    possibleOutsiderAdjustments.Add(GetOutsiderAdjustments(character).ToList());
                }
            }

            var possibleCumulativeOutsiderAdjustments = new List<int>();
            CalculateCumulativeOutsiderAdjustment(possibleOutsiderAdjustments, 0, 0, possibleCumulativeOutsiderAdjustments);

            int maximumOutsiders = GetMaximumOutsiderCount();
            return possibleCumulativeOutsiderAdjustments.Distinct()
                                                        .Select(adjustment => Clamp(baseOutsiders + adjustment, 0, maximumOutsiders))
                                                        .Distinct();
        }

        private static int GetRequiredMinions(int playerCount)
        {
            return playerCount switch
            {
                <= 9 => 1,
                <= 12 => 2,
                _ => 3
            };
        }

        private int GetBaseOutsiderCount(int playerCount)
        {
            return playerCount switch
            {
                7 => 0,
                8 => 1,
                9 => 2,
                10 => 0,
                11 => 1,
                12 => 2,
                13 => 0,
                14 => 1,
                _ => 2
            };
        }

        private int GetMaximumOutsiderCount()
        {
            return script.OfCharacterType(CharacterType.Outsider).Count();
        }

        private static IEnumerable<int> GetOutsiderAdjustments(Character character)
        {
            switch (character)
            {
                case Character.Baron:
                    yield return +2;
                    break;

                case Character.Godfather:
                    yield return -1;
                    yield return +1;
                    break;

                case Character.Balloonist:
                    yield return 0;
                    yield return +1;
                    break;
            }
        }

        private static void CalculateCumulativeOutsiderAdjustment(List<IReadOnlyCollection<int>> possibleOutsiderAdjustments, int index, int currentCumulativeOutsiderAdjustment, List<int> possibleCumulativeOutsiderAdjustments)
        {
            if (index >= possibleOutsiderAdjustments.Count)
            {
                possibleCumulativeOutsiderAdjustments.Add(currentCumulativeOutsiderAdjustment);
                return;
            }

            foreach (var outsiderAdjustment in possibleOutsiderAdjustments[index])
            {
                CalculateCumulativeOutsiderAdjustment(possibleOutsiderAdjustments, index + 1, currentCumulativeOutsiderAdjustment + outsiderAdjustment, possibleCumulativeOutsiderAdjustments);
            }
        }

        private static int Clamp(int value, int minimum, int maximum)
        {
            return value < minimum ? minimum 
                 : value > maximum ? maximum : value;
        }

        private readonly Random random;
        private readonly IReadOnlyCollection<Character> script;
        private readonly Func<Character, bool> isCharacterSelected;
    }
}
