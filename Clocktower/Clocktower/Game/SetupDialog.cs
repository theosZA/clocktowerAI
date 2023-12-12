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
            if (IsCharacterSelected(Character.Drunk))
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
            for (int column = 0; column < characterTypes.Length; column++)
            {
                var characterType = characterTypes[column];
                var setup = new SetupForCharacterType(characterType, Script.OfCharacterType(characterType), IsCharacterSelected, UpdateCounters);
                setupForCharacterType.Add(characterType, setup);

                charactersPanel.Controls.Add(setup.Heading, column, 0);
                charactersPanel.Controls.Add(setup.Counter, column, 1);
                foreach (var characterControl in setup.CharacterControls.Select((control, i) => (control, i)))
                {
                    charactersPanel.Controls.Add(characterControl.control, column, characterControl.i + 2);
                }
            }

            UpdateCounters();
        }

        private bool IsCharacterSelected(Character character)
        {
            return setupForCharacterType.Any(setup => setup.Value.SelectedCharacters.Contains(character));
        }

        private void UpdateCounters()
        {
            foreach (var setup in setupForCharacterType)
            {
                setup.Value.UpdateCounter(PlayerCount);
            }

            startButton.Enabled = setupForCharacterType.All(setup => setup.Value.IsCountOkay(PlayerCount))
                               && GetRequiredTotal() == setupForCharacterType.Sum(setup => setup.Value.SelectedCount);
        }

        private int GetRequiredTotal()
        {
            int requiredPlayers = PlayerCount;
            if (IsCharacterSelected(Character.Drunk))
            {
                ++requiredPlayers;
            }
            return requiredPlayers;
        }

        private void UpdateCounters(object? sender, EventArgs e)
        {
            UpdateCounters();
        }

        private void RandomizeBag(object sender, EventArgs e)
        {
            // Maximize the outsider count 90% of the time.
            bool maximizeOutsiders = random.Next(10) > 0;
            bool maximizeTownsfolk = !maximizeOutsiders;

            // We need to randomize the characters in this specific order to ensure the counts are correct.
            setupForCharacterType[CharacterType.Demon].RandomizeSelection(PlayerCount, maximizeCount: true, random);
            setupForCharacterType[CharacterType.Minion].RandomizeSelection(PlayerCount, maximizeCount: true, random);
            setupForCharacterType[CharacterType.Outsider].RandomizeSelection(PlayerCount, maximizeOutsiders, random);
            setupForCharacterType[CharacterType.Townsfolk].RandomizeSelection(PlayerCount, maximizeTownsfolk, random);

            UpdateCounters();
        }

        private void StartGame(object sender, EventArgs e)
        {
            var bag = setupForCharacterType.SelectMany(setup => setup.Value.SelectedCharacters)
                                           .Where(character => character != Character.Drunk)
                                           .ToList();
            bag.Shuffle(random);
            Characters = bag.ToArray();

            DialogResult = DialogResult.OK;
        }

        private readonly Random random;

        private readonly CharacterType[] characterTypes = new[] { CharacterType.Townsfolk, CharacterType.Outsider, CharacterType.Minion, CharacterType.Demon };

        private readonly Dictionary<CharacterType, SetupForCharacterType> setupForCharacterType = new();
    }
}
