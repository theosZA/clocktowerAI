using Clocktower.Agent;
using Clocktower.Game;
using Clocktower.Selection;
using Clocktower.Setup;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class SelectionOfKazaliMinions : IGameEvent
    {
        public SelectionOfKazaliMinions(IStoryteller storyteller, Grimoire grimoire, IReadOnlyCollection<Character> script)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.script = script;
        }

        public async Task RunEvent()
        {
            foreach (var kazali in grimoire.PlayersForWhomWeShouldRunAbility(Character.Kazali))
            {
                var kazaliMinionsSelection = await GetKazaliMinionsSelection(kazali);
                for (int i = 0; i < kazaliMinionsSelection.MinionCount; i++)
                {
                    var (player, character) = kazaliMinionsSelection.Minions.ElementAt(i);
                    await UpdatePlayerWithMinionCharacter(player, character, kazali, kazaliMinionsSelection);
                }
            }
        }

        private async Task<KazaliMinionsSelection> GetKazaliMinionsSelection(Player kazali)
        {
            int minionCount = CharacterTypeDistribution.GetBaseMinionCount(grimoire.Players.Count);
            var possiblePlayers = grimoire.Players.Where(player => player != kazali).ToList();
            var minionCharacters = script.Where(character => character.CharacterType() == CharacterType.Minion).ToList();

            var characterLimitations = new Dictionary<Character, IReadOnlyCollection<Player>>();
            if (script.Contains(Character.Marionette))
            {
                var players = grimoire.Players.ToList();
                int kazaliIndex = players.IndexOf(kazali);
                var neighbours = new[]
                {
                    players[(kazaliIndex + 1) % players.Count],
                    players[(kazaliIndex + players.Count - 1) % players.Count]
                };
                characterLimitations.Add(Character.Marionette, neighbours);
            }

            var kazaliMinionSelection = new KazaliMinionsSelection(minionCount, possiblePlayers, minionCharacters, characterLimitations);
            await kazali.Agent.RequestSelectionOfKazaliMinions(kazaliMinionSelection);
            storyteller.KazaliMinions(kazali, kazaliMinionSelection.Minions);

            return kazaliMinionSelection;
        }

        private async Task UpdatePlayerWithMinionCharacter(Player player, Character minionCharacter, Player kazali, KazaliMinionsSelection kazaliMinionsSelection)
        {
            if (player.HasHealthyAbility(Character.Soldier))
            {
                // A Soldier selected by the Kazali instead chooses which not-in-play Minion to become.
                var soldierMinion = await UpdateSoldierAsMinion(player, kazaliMinionsSelection);

                // If this Minion was picked for any of the Kazali's subsequent choices, the Kazali player will have to choose a new Minion character.
                if (kazaliMinionsSelection.Minions.Any(selection => selection.character == soldierMinion && selection.player != player))
                {
                    var matchingSelection = kazaliMinionsSelection.Minions.First(selection => selection.character == soldierMinion && selection.player != player);
                    await UpdateKazaliSelectionWithNewMinion(kazali, matchingSelection.player, matchingSelection.character, kazaliMinionsSelection);
                }

                return;
            }

            await grimoire.ChangeCharacter(player, minionCharacter);
            storyteller.AssignCharacter(player);
        }

        private async Task<Character> UpdateSoldierAsMinion(Player soldier, KazaliMinionsSelection kazaliMinionsSelection)
        {
            var currentMinionCharacters = grimoire.Players.WithCharacterType(CharacterType.Minion).Select(player => player.RealCharacter);
            var availableMinionCharacters = kazaliMinionsSelection.MinionCharacters.Except(currentMinionCharacters);

            var newMinion = await soldier.Agent.RequestChoiceOfMinionForSoldierSelectedByKazali(availableMinionCharacters);
            kazaliMinionsSelection.ReplaceMinionPick(soldier, newMinion);

            storyteller.KazaliSoldierMinion(soldier, newMinion);
            await grimoire.ChangeCharacter(soldier, newMinion);
            storyteller.AssignCharacter(soldier);

            return newMinion;
        }

        private async Task UpdateKazaliSelectionWithNewMinion(Player kazali, Player pickedPlayer, Character pickedMinion, KazaliMinionsSelection kazaliMinionsSelection)
        {
            var pickedMinions = kazaliMinionsSelection.Minions.Where(selection => selection.player != pickedPlayer)
                                                              .Select(selection => selection.character);
            var limitatedMinions = kazaliMinionsSelection.CharacterLimitations.Where(limitation => !limitation.Value.Contains(pickedPlayer))
                                                                              .Select(limitation => limitation.Key);
            var availableMinions = kazaliMinionsSelection.MinionCharacters.Except(pickedMinions)
                                                                          .Except(limitatedMinions);
            var newMinionPick = await kazali.Agent.RequestNewKazaliMinion(pickedPlayer, pickedMinion, availableMinions);
            kazaliMinionsSelection.ReplaceMinionPick(pickedPlayer, newMinionPick);

            storyteller.NewKazaliMinion(kazali, pickedPlayer, pickedMinion, newMinionPick);
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IReadOnlyCollection<Character> script;
    }
}
