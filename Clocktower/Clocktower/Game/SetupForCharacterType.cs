using System.ComponentModel;

namespace Clocktower.Game
{
    internal class SetupForCharacterType
    {
        public Control Heading { get; private set; }

        public Control Counter { get; } = new Label();

        public IEnumerable<Control> CharacterControls => checkBoxes.Select(kvp => kvp.Value);

        public int SelectedCount => checkBoxes.Where(kvp => kvp.Value.Checked).Count();

        public IEnumerable<Character> SelectedCharacters => checkBoxes.Where(kvp => kvp.Value.Checked).Select(kvp => kvp.Key);

        public SetupForCharacterType(CharacterType characterType, IEnumerable<Character> characters, Func<Character, bool> isCharacterSelected, EventHandler onCharacterSelectedChanged)
        {
            this.characterType = characterType;
            this.isCharacterSelected = isCharacterSelected;

            Heading = new Label
            { 
                Text = characterType.ToString(),
                Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold)
            };

            foreach (var character in characters)
            {
                var checkbox = new CheckBox
                {
                    Text = TextUtilities.CharacterToText(character),
                    Dock = DockStyle.Fill
                };
                checkbox.CheckedChanged += onCharacterSelectedChanged;

                checkBoxes.Add(character, checkbox);
            }

            if (characterType == CharacterType.Demon && checkBoxes.Count == 1)
            {
                var singleDemonCheckBox = checkBoxes.First().Value;
                singleDemonCheckBox.Checked = true;
                singleDemonCheckBox.Enabled = false;
            }
        }

        public void UpdateCounter(int playerCount)
        {
            Counter.Text = $"{SelectedCount} of {string.Join('/', GetRequiredCount(playerCount))}";
        }

        public bool IsCountOkay(int playerCount)
        {
            return GetRequiredCount(playerCount).Contains(SelectedCount);
        }

        public void RandomizeSelection(int playerCount, bool maximizeCount, Random random, IEnumerable<Character> requiredCharacters)
        {
            foreach (var checkbox in checkBoxes)
            {
                checkbox.Value.Checked = false;
            }

            foreach (var requiredCharacter in requiredCharacters)
            {
                ForceCheck(requiredCharacter);
            }

            int selectedCount = maximizeCount ? GetRequiredCount(playerCount).Max() : GetRequiredCount(playerCount).Min();
            foreach (var checkbox in checkBoxes.Select(kvp => kvp.Value)
                                               .Where(checkBox => !checkBox.Checked)
                                               .ToList()
                                               .RandomPickN(selectedCount - SelectedCount, random))
            {
                checkbox.Checked = true;
            }
        }

        public void ForceCheck(Character character)
        {
            if (checkBoxes.TryGetValue(character, out var checkBox))
            {
                checkBox.Checked = true;
                checkBox.Enabled = false;
            }
        }

        private IEnumerable<int> GetRequiredCount(int playerCount)
        {
            return characterType switch
            {
                CharacterType.Townsfolk => GetRequiredTownsfolk(playerCount),
                CharacterType.Outsider => GetRequiredOutsiders(playerCount),
                CharacterType.Minion => new[] { GetRequiredMinions(playerCount) },
                CharacterType.Demon => new[] { 1 },
                _ => throw new InvalidEnumArgumentException(nameof(characterType))
            };
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

        private IEnumerable<int> GetRequiredOutsiders(int playerCount)
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

            if (isCharacterSelected(Character.Godfather))
            {
                if (baseOutsiders > 0)
                {
                    yield return baseOutsiders - 1;
                }
                yield return baseOutsiders + 1;
            }
            else
            {
                yield return baseOutsiders;
            }
        }

        private IEnumerable<int> GetRequiredTownsfolk(int playerCount)
        {
            int demonCount = 1;
            int minionCount = GetRequiredMinions(playerCount);

            foreach (int outsiderCount in GetRequiredOutsiders(playerCount))
            {
                int townsfolkCount = playerCount - (outsiderCount + minionCount + demonCount);

                if (isCharacterSelected(Character.Drunk))
                {
                    ++townsfolkCount;
                }

                yield return townsfolkCount;
            }
        }

        private readonly CharacterType characterType;
        private readonly Func<Character, bool> isCharacterSelected;

        private readonly Dictionary<Character, CheckBox> checkBoxes = new();
    }
}
