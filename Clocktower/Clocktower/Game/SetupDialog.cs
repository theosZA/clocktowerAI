using Clocktower.Events;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    public partial class SetupDialog : Form, IGameSetup
    {
        /// <summary>
        /// The characters available in this game.
        /// </summary>
        public IReadOnlyCollection<Character> Script { get; private set; }

        /// <summary>
        /// The number of players in the game.
        /// </summary>
        public int PlayerCount => (int)playerCountUpDown.Value;

        /// <summary>
        /// The characters assigned to each seat (0...n-1).
        /// </summary>
        public Character[] Characters { get; private set; }

        /// <summary>
        /// Additional events that have to be run to complete setup.
        /// These are typically events that will require storyteller intervention, e.g. assigning the Drunk.
        /// </summary>
        public IEnumerable<IGameEvent> BuildAdditionalSetupEvents(IStoryteller storyteller, Grimoire grimoire)
        {
            if (CharacterChecked(Character.Drunk))
            {
                yield return new AssignDrunk(storyteller, grimoire);
            }
        }

        public SetupDialog(Random random)
        {
            InitializeComponent();

            this.random = random;

            Characters = Array.Empty<Character>();

            // "A Simple Matter"
            Script = new List<Character>
            {   // Townsfolk
                Character.Steward,
                Character.Investigator,
                Character.Librarian,
                Character.Shugenja,
                Character.Empath,
                Character.Fortune_Teller,
                Character.Undertaker,
                Character.Monk,
                Character.Fisherman,
                Character.Slayer,
                Character.Philosopher,
                Character.Soldier,
                Character.Ravenkeeper,
                // Outsiders
                Character.Tinker,
                Character.Sweetheart,
                Character.Recluse,
                Character.Drunk,
                // Minions
                Character.Godfather,
                Character.Poisoner,
                Character.Assassin,
                Character.Scarlet_Woman,
                // Demons
                Character.Imp
            };

            // Populate grid with script characters.

            charactersPanel.Controls.Add(new Label { Text = "Townsfolk" }, 0, 0);
            charactersPanel.Controls.Add(new Label { Text = "Outsiders" }, 1, 0);
            charactersPanel.Controls.Add(new Label { Text = "Minions" }, 2, 0);
            charactersPanel.Controls.Add(new Label { Text = "Demon" }, 3, 0);

            charactersPanel.Controls.Add(townsfolkCounter, 0, 1);
            charactersPanel.Controls.Add(outsidersCounter, 1, 1);
            charactersPanel.Controls.Add(minionsCounter, 2, 1);
            charactersPanel.Controls.Add(demonsCounter, 3, 1);

            foreach (var character in Script)
            {
                AddCharacter(character);
            }
            if (demonsCheckboxes.Count == 1)
            {
                demonsCheckboxes[0].Checked = true;
                demonsCheckboxes[0].Enabled = false;
            }

            UpdateCounters();
        }

        private void AddCharacter(Character character)
        {
            switch ((int)character)
            {
                case < 1000:
                    AddCharacter(character, townsfolkCheckboxes, charactersPanel.Controls, 0);
                    break;

                case < 2000:
                    AddCharacter(character, outsidersCheckboxes, charactersPanel.Controls, 1);
                    break;

                case < 3000:
                    AddCharacter(character, minionsCheckboxes, charactersPanel.Controls, 2);
                    break;

                default:
                    AddCharacter(character, demonsCheckboxes, charactersPanel.Controls, 3);
                    break;
            }
        }

        private void AddCharacter(Character character, IList<CheckBox> checkboxesForCharacterType, TableLayoutControlCollection tableControls, int column)
        {
            var checkbox = new CheckBox
            {
                Text = TextUtilities.CharacterToText(character),
                Dock = DockStyle.Fill
            };
            checkbox.CheckedChanged += UpdateCounters;

            checkboxes.Add(character, checkbox);
            checkboxesForCharacterType.Add(checkbox);
            tableControls.Add(checkbox, column, checkboxesForCharacterType.Count + 1);
        }

        private void UpdateCounters()
        {
            int townsfolkCount = townsfolkCheckboxes.Sum(checkbox => checkbox.Checked ? 1 : 0);
            int outsiderCount = outsidersCheckboxes.Sum(checkbox => checkbox.Checked ? 1 : 0);
            int minionsCount = minionsCheckboxes.Sum(checkbox => checkbox.Checked ? 1 : 0);
            int demonsCount = demonsCheckboxes.Sum(checkbox => checkbox.Checked ? 1 : 0);

            UpdateCounter(townsfolkCounter, townsfolkCount, GetRequiredTownsfolk());
            UpdateCounter(outsidersCounter, outsiderCount, GetRequiredOutsiders());
            UpdateCounter(minionsCounter, minionsCount, new[] { GetRequiredMinions() });
            UpdateCounter(demonsCounter, demonsCount, new[] { 1 });

            startButton.Enabled = (GetRequiredTownsfolk().Contains(townsfolkCount) &&
                                   GetRequiredOutsiders().Contains(outsiderCount) &&
                                   GetRequiredMinions() == minionsCount &&
                                   1 == demonsCount &&
                                   GetRequiredTotal() == townsfolkCount + outsiderCount + minionsCount + demonsCount);
        }

        private static void UpdateCounter(Control control, int selectedCount, IEnumerable<int> requiredCount)
        {
            control.Text = $"{selectedCount} of {string.Join('/', requiredCount)}";
        }

        private int GetRequiredMinions()
        {
            return PlayerCount switch
            {
                <= 9 => 1,
                <= 12 => 2,
                _ => 3
            };
        }

        private IEnumerable<int> GetRequiredOutsiders()
        {
            int baseOutsiders = PlayerCount switch
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

            if (CharacterChecked(Character.Godfather))
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

        private IEnumerable<int> GetRequiredTownsfolk()
        {
            int demonCount = 1;
            int minionCount = GetRequiredMinions();

            foreach (int outsiderCount in GetRequiredOutsiders())
            {
                int townsfolkCount = PlayerCount - (outsiderCount + minionCount + demonCount);

                if (CharacterChecked(Character.Drunk))
                {
                    ++townsfolkCount;
                }

                yield return townsfolkCount;
            }
        }

        private int GetRequiredTotal()
        {
            int requiredPlayers = PlayerCount;
            if (CharacterChecked(Character.Drunk))
            {
                ++requiredPlayers;
            }
            return requiredPlayers;
        }

        private bool CharacterChecked(Character character)
        {
            return checkboxes.TryGetValue(character, out var checkbox) && checkbox.Checked;
        }

        private void UpdateCounters(object? sender, EventArgs e)
        {
            UpdateCounters();
        }

        private void StartGame(object sender, EventArgs e)
        {
            var bag = checkboxes.Where(kvp => kvp.Value.Checked)
                                .Select(kvp => kvp.Key)
                                .Where(character => character != Character.Drunk)
                                .ToList();
            bag.Shuffle(random);
            Characters = bag.ToArray();

            DialogResult = DialogResult.OK;
        }

        private readonly Random random;

        private readonly Control townsfolkCounter = new Label();
        private readonly Control outsidersCounter = new Label();
        private readonly Control minionsCounter = new Label();
        private readonly Control demonsCounter = new Label();

        private readonly List<CheckBox> townsfolkCheckboxes = new();
        private readonly List<CheckBox> outsidersCheckboxes = new();
        private readonly List<CheckBox> minionsCheckboxes = new();
        private readonly List<CheckBox> demonsCheckboxes = new();

        private readonly Dictionary<Character, CheckBox> checkboxes = new();
    }
}
