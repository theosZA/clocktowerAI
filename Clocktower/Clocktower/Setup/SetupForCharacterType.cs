using Clocktower.Game;

namespace Clocktower.Setup
{
    internal class SetupForCharacterType
    {
        public Control Heading { get; private set; }

        public Control Counter { get; } = new Label();

        public IEnumerable<Control> CharacterControls => checkBoxes.Select(kvp => kvp.Value);

        public int SelectedCount => checkBoxes.Where(kvp => kvp.Value.Checked).Count();

        public IEnumerable<Character> SelectedCharacters => checkBoxes.Where(kvp => kvp.Value.Checked).Select(kvp => kvp.Key);

        public SetupForCharacterType(CharacterType characterType, IEnumerable<Character> characters, CharacterTypeDistribution characterTypeDistribution, EventHandler onCharacterSelectedChanged)
        {
            this.characterType = characterType;
            this.characterTypeDistribution = characterTypeDistribution;

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
        }

        public void Clear()
        {
            foreach (var checkbox in checkBoxes)
            {
                checkbox.Value.Checked = false;
            }
        }

        public void SetRequiredCharacters(IEnumerable<Character> requiredCharacters)
        {
            foreach (var requiredCharacter in requiredCharacters)
            {
                ForceCheck(requiredCharacter);
            }
        }

        public void UpdateCounter(int playerCount)
        {
            Counter.Text = $"{SelectedCount} of {string.Join('/', characterTypeDistribution.GetPossibleCounts(characterType, playerCount))}";
        }

        public bool IsCountOkay(int playerCount)
        {
            return characterTypeDistribution.GetPossibleCounts(characterType, playerCount).Contains(SelectedCount);
        }

        public void RandomizeSelection(int count, Random random, IEnumerable<Character> requiredCharacters)
        {
            Clear();
            SetRequiredCharacters(requiredCharacters);

            foreach (var checkbox in checkBoxes.Select(kvp => kvp.Value)
                                               .Where(checkBox => !checkBox.Checked)
                                               .ToList()
                                               .RandomPickN(count - SelectedCount, random))
            {
                checkbox.Checked = true;
            }
        }

        public void AddRandomCharacter(Random random, IEnumerable<Character> excludedCharacters)
        {
            var checkbox = checkBoxes.ExceptBy(excludedCharacters, kvp => kvp.Key)
                                     .Select(kvp => kvp.Value)
                                     .Where(checkBox => !checkBox.Checked)
                                     .ToList()
                                     .RandomPick(random);
            checkbox.Checked = true;
        }

        public void RemoveRandomCharacter(Random random, IEnumerable<Character> excludedCharacters)
        {
            var checkbox = checkBoxes.ExceptBy(excludedCharacters, kvp => kvp.Key)
                                     .Select(kvp => kvp.Value)
                                     .Where(checkBox => checkBox.Checked)
                                     .ToList()
                                     .RandomPick(random);
            checkbox.Checked = false;
        }

        public void ForceCheck(Character character)
        {
            if (checkBoxes.TryGetValue(character, out var checkBox))
            {
                checkBox.Checked = true;
                checkBox.Enabled = false;
            }
        }

        private readonly CharacterType characterType;
        private readonly CharacterTypeDistribution characterTypeDistribution;

        private readonly Dictionary<Character, CheckBox> checkBoxes = new();
    }
}
