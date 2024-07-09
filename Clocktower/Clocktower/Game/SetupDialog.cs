namespace Clocktower.Game
{
    public partial class SetupDialog : Form, IGameSetup
    {
        /// <summary>
        /// The name of the script defining the characters available in this game.
        /// </summary>
        public string ScriptName { get; private set; }

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

        public SetupDialog(string scriptFileName, Random random, IReadOnlyCollection<Alignment?> forcedAlignments, IReadOnlyCollection<Character?> forcedCharacters)
        {
            InitializeComponent();

            ScriptName = Path.GetFileNameWithoutExtension(scriptFileName);
            Script = ClocktowerScriptReader.ReadScriptFromFile(scriptFileName).ToList();
            Characters = Array.Empty<Character>();

            this.Text = ScriptName;
            this.random = random;
            this.forcedAlignments = forcedAlignments;
            this.forcedCharacters = forcedCharacters;

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

            // Toggle forced characters.
            foreach (var character in forcedCharacters)
            {
                if (character.HasValue)
                {
                    setupForCharacterType[character.Value.CharacterType()].ForceCheck(character.Value);
                }
            }

            UpdateCounters();
        }

        /// <summary>
        /// Checks if the given character is to be included in the game even if that character hasn't been assigned to a seat yet.
        /// </summary>
        /// <param name="character">Character to check if it is to be included in the game.</param>
        /// <returns>True if the given character is to be included in the game.</returns>
        public bool IsCharacterSelected(Character character)
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
            var forcedCharacters = this.forcedCharacters.Take(PlayerCount).Where(character => character.HasValue).Select(character => character!.Value).ToList();
            setupForCharacterType[CharacterType.Demon].RandomizeSelection(PlayerCount, maximizeCount: true, random, forcedCharacters);
            setupForCharacterType[CharacterType.Minion].RandomizeSelection(PlayerCount, maximizeCount: true, random, forcedCharacters);
            setupForCharacterType[CharacterType.Outsider].RandomizeSelection(PlayerCount, maximizeOutsiders, random, forcedCharacters);
            setupForCharacterType[CharacterType.Townsfolk].RandomizeSelection(PlayerCount, maximizeTownsfolk, random, forcedCharacters);

            UpdateCounters();
        }

        private void StartGame(object sender, EventArgs e)
        {
            var bag = setupForCharacterType.SelectMany(setup => setup.Value.SelectedCharacters)
                                           .Where(character => character != Character.Drunk)
                                           .ToList();
            var characters = new Character?[bag.Count];

            // Assign fixed characters first.
            for (int i = 0; i < characters.Length; ++i)
            {
                var character = forcedCharacters.Skip(i).FirstOrDefault();
                if (character.HasValue && bag.Contains(character.Value))
                {
                    bag.Remove(character.Value);
                    characters[i] = character;
                }
            }

            // Assign fixed alignments next.
            var charactersByAlignment = bag.Select(character => (character.Alignment(), character))
                                           .GroupBy(k => k.Item1)
                                           .ToDictionary(group => group.Key, group => group.Select(pair => pair.character).ToList());
            for (int i = 0; i < characters.Length; ++i)
            {
                if (!characters[i].HasValue)
                {
                    var alignment = forcedAlignments.Skip(i).FirstOrDefault();
                    if (alignment.HasValue)
                    {
                        var possibleCharacters = charactersByAlignment[alignment.Value];
                        var character = possibleCharacters.RandomPick(random);
                        possibleCharacters.Remove(character);
                        characters[i] = character;
                    }
                }
            }

            // Assign the remaining characters randomly.
            var remainingCharacters = charactersByAlignment.SelectMany(pair => pair.Value).ToList();
            remainingCharacters.Shuffle(random);
            var remainingCharactersQueue = new Queue<Character>(remainingCharacters);
            Characters = characters.Select(character => character ?? remainingCharactersQueue.Dequeue()).ToArray();

            DialogResult = DialogResult.OK;
        }

        private readonly Random random;
        private readonly IReadOnlyCollection<Alignment?> forcedAlignments;
        private readonly IReadOnlyCollection<Character?> forcedCharacters;

        private readonly CharacterType[] characterTypes = new[] { CharacterType.Townsfolk, CharacterType.Outsider, CharacterType.Minion, CharacterType.Demon };

        private readonly Dictionary<CharacterType, SetupForCharacterType> setupForCharacterType = new();
    }
}
