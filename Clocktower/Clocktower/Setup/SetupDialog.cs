using Clocktower.Setup;

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

        public SetupDialog(string? scriptFileName, Random random, IReadOnlyCollection<Alignment?> forcedAlignments, IReadOnlyCollection<Character?> forcedCharacters)
        {
            InitializeComponent();

            if (scriptFileName == null)
            {   // If no script name is passed in, we use "Whale Bucket", the script that includes all characters.
                ScriptName = "Whale Bucket";
                Script = Enum.GetValues<Character>().OrderBy(character => character.ToString()).ToList();
            }
            else
            {
                ScriptName = Path.GetFileNameWithoutExtension(scriptFileName);
                Script = ClocktowerScriptReader.ReadScriptFromFile(scriptFileName).ToList();
            }

            Characters = Array.Empty<Character>();

            this.Text = ScriptName;
            this.random = random;
            this.forcedAlignments = forcedAlignments;
            this.forcedCharacters = forcedCharacters;

            characterTypeDistribution = new(random, Script, IsCharacterSelected);

            // Populate grid with script characters.
            for (int column = 0; column < characterTypes.Length; column++)
            {
                var characterType = characterTypes[column];
                var setup = new SetupForCharacterType(characterType, Script.OfCharacterType(characterType), characterTypeDistribution, UpdateCounters);
                setupForCharacterType.Add(characterType, setup);

                charactersPanel.Controls.Add(setup.Heading, column, 0);
                charactersPanel.Controls.Add(setup.Counter, column, 1);
                foreach (var characterControl in setup.CharacterControls.Select((control, i) => (control, i)))
                {
                    charactersPanel.Controls.Add(characterControl.control, column, characterControl.i + 2);
                }
            }

            SetRequiredCharacters();
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

        /// <summary>
        /// Checks if it's possible for the specified character to actually be the Drunk without breaking the modifications to the Outsider count.
        /// </summary>
        /// <param name="character">Character to check if they could be the Drunk instead.</param>
        /// <returns>True if this character can be turned into the Drunk.</returns>
        public bool CanCharacterBeTheDrunk(Character character)
        {
            if (character.CharacterType() != CharacterType.Townsfolk)
            {
                return false;
            }

            if (character != Character.Balloonist)
            {
                return true;
            }

            // Is the current Outsider count possible without the Balloonist modification to Outsider count?
            var possibleOutsiderCounts = characterTypeDistribution.GetPossibleOutsiderCountsExcludingCharacter(PlayerCount, Character.Balloonist);
            return possibleOutsiderCounts.Contains(setupForCharacterType[CharacterType.Outsider].SelectedCount);
        }

        private void SetRequiredCharacters()
        {
            var possibleDemons = Script.OfCharacterType(CharacterType.Demon).ToList();
            if (possibleDemons.Count == 1)
            {
                setupForCharacterType[CharacterType.Demon].ForceCheck(possibleDemons[0]);
            }

            var actualForcedCharacters = this.forcedCharacters.Take(PlayerCount).Where(character => character.HasValue).Select(character => character!.Value).ToList();
            foreach (var characterType in characterTypes)
            {
                setupForCharacterType[characterType].SetRequiredCharacters(actualForcedCharacters);
            }
        }

        private void UpdateCounters()
        {
            foreach (var setup in setupForCharacterType)
            {
                setup.Value.UpdateCounter(PlayerCount);
            }

            bool huntsmanNeedsDamsel = IsCharacterSelected(Character.Huntsman) && !IsCharacterSelected(Character.Damsel);
            setupForCharacterType[CharacterType.Outsider].SetColor(Character.Damsel, huntsmanNeedsDamsel ? Color.Red : Color.Black);

            startButton.Enabled = setupForCharacterType.All(setup => setup.Value.IsCountOkay(PlayerCount))
                               && GetRequiredTotal() == setupForCharacterType.Sum(setup => setup.Value.SelectedCount)
                               && !huntsmanNeedsDamsel;
        }

        private int GetRequiredTotal()
        {
            int requiredPlayers = PlayerCount;
            if (IsCharacterSelected(Character.Drunk))
            {
                ++requiredPlayers;
            }
            if (IsCharacterSelected(Character.Marionette))
            {
                ++requiredPlayers;
            }
            return requiredPlayers;
        }

        private void UpdateCounters(object? sender, EventArgs e)
        {
            SetRequiredCharacters();
            UpdateCounters();
        }

        private void RandomizeBag(object sender, EventArgs e)
        {
            foreach (var characterType in characterTypes)
            {
                setupForCharacterType[characterType].Clear();
            }

            var forcedCharacters = this.forcedCharacters.Take(PlayerCount).Where(character => character.HasValue).Select(character => character!.Value).ToList();

            int demonCount = 1;
            setupForCharacterType[CharacterType.Demon].RandomizeSelection(demonCount, random, forcedCharacters);

            int minionCount = characterTypeDistribution.GetMinionCount(PlayerCount);
            setupForCharacterType[CharacterType.Minion].RandomizeSelection(minionCount, random, forcedCharacters);

            // At this point we need to determine the distribution between Townsfolk and Outsiders. This is complicated by the fact that two Townsfolk (Balloonist
            // and Huntsman) can potentially change the Outsider count (if they aren't the Drunk). What we'll do is assume for the moment that there will be no modifications
            // to the Outsider count from Townsfolk and see what Townsfolk characters are drawn.
            // - If one of them is the Balloonist then we'll randomly determine if they are applying a +0 or +1 to the Outsider count, and if it's a +1 modification then
            //   we'll remove a random non-Balloonist Townsfolk.
            // - If one of them is the Huntsman then we'll randomly pick the Outsiders and see if the Damsel is added. If not, then we'll add the Damsel and remove a
            //   non-Balloonist, non-Huntsman Townsfolk.

            int outsiderModification = characterTypeDistribution.GetRandomOutsiderModificationByEvil();
            int townsfolkCount = characterTypeDistribution.GetTownsfolkCount(PlayerCount, outsiderModification);
            setupForCharacterType[CharacterType.Townsfolk].RandomizeSelection(townsfolkCount, random, forcedCharacters);

            outsiderModification += characterTypeDistribution.GetRandomOutsiderModificationByGood();
            int outsiderCount = characterTypeDistribution.GetOutsiderCount(PlayerCount, outsiderModification);
            setupForCharacterType[CharacterType.Outsider].RandomizeSelection(outsiderCount, random, forcedCharacters);

            int newTownsfolkCount = characterTypeDistribution.GetTownsfolkCount(PlayerCount, outsiderModification);
            while (newTownsfolkCount < setupForCharacterType[CharacterType.Townsfolk].SelectedCount)
            {   // Remove townsfolk (other than Balloonist).
                setupForCharacterType[CharacterType.Townsfolk].RemoveRandomCharacter(random, new[] { Character.Balloonist });
            }
            while (newTownsfolkCount > setupForCharacterType[CharacterType.Townsfolk].SelectedCount)
            {   // Add townsfolk (other than Balloonist).
                setupForCharacterType[CharacterType.Townsfolk].AddRandomCharacter(random, new[] { Character.Balloonist });
            }

            if (IsCharacterSelected(Character.Huntsman) && !IsCharacterSelected(Character.Damsel))
            {
                setupForCharacterType[CharacterType.Outsider].SelectCharacter(Character.Damsel);
                setupForCharacterType[CharacterType.Townsfolk].RemoveRandomCharacter(random, new[] { Character.Balloonist, Character.Huntsman });
            }

            UpdateCounters();
        }

        private void StartGame(object sender, EventArgs e)
        {
            var bag = setupForCharacterType.SelectMany(setup => setup.Value.SelectedCharacters)
                                           .Where(character => character != Character.Drunk && character != Character.Marionette)
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

            do
            {
                // Assign the remaining characters randomly.
                var remainingCharacters = charactersByAlignment.SelectMany(pair => pair.Value).ToList();
                remainingCharacters.Shuffle(random);
                var remainingCharactersQueue = new Queue<Character>(remainingCharacters);
                Characters = characters.Select(character => character ?? remainingCharactersQueue.Dequeue()).ToArray();
            }
            while (!IsSetupOk());

            DialogResult = DialogResult.OK;
        }

        private bool IsSetupOk()
        {
            if (!setupForCharacterType[CharacterType.Minion].SelectedCharacters.Contains(Character.Marionette))
            {
                return true;
            }

            // Marionette can replace a Townsfolk or Outsider adjacent to the Demon.
            for (int i = 0; i < Characters.Length; ++i)
            {
                if (Characters[i].CharacterType() == CharacterType.Demon || Characters[i] == Character.Recluse)
                {
                    var neighbourA = Characters[(i + 1) % Characters.Length];
                    if (neighbourA.CharacterType() == CharacterType.Townsfolk || neighbourA.CharacterType() == CharacterType.Outsider)
                    {
                        return true;
                    }
                    var neighbourB = Characters[(i + Characters.Length - 1) % Characters.Length];
                    if (neighbourB.CharacterType() == CharacterType.Townsfolk || neighbourB.CharacterType() == CharacterType.Outsider)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private readonly Random random;
        private readonly IReadOnlyCollection<Alignment?> forcedAlignments;
        private readonly IReadOnlyCollection<Character?> forcedCharacters;

        private readonly CharacterType[] characterTypes = new[] { CharacterType.Townsfolk, CharacterType.Outsider, CharacterType.Minion, CharacterType.Demon };

        private readonly CharacterTypeDistribution characterTypeDistribution;
        private readonly Dictionary<CharacterType, SetupForCharacterType> setupForCharacterType = new();
    }
}
