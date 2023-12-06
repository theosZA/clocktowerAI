using Clocktower.Game;
using Clocktower.Observer;

namespace Clocktower
{
    public partial class StorytellerForm : Form
    {
        public IGameObserver Observer { get; private set; }

        public StorytellerForm()
        {
            InitializeComponent();
            Observer = new RichTextBoxObserver(outputText)
            {
                StorytellerView = true
            };
        }

        public void AssignCharacter(Player player)
        {
            if (player.RealCharacter == null || player.Character == null || player.RealAlignment == null || player.Alignment == null)
            {
                throw new InvalidOperationException("Player has not been assigned a character/alignment");
            }
            if (player.Character == player.RealCharacter)
            {
                outputText.AppendFormattedText("%p is the %c.\n", player, player.Character);
            }
            else
            {
                outputText.AppendFormattedText("%p believes they are the %c but they are actually the %c.\n", player, player.Character, player.RealCharacter);
            }
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            if (fellowMinions.Any())
            {
                outputText.AppendFormattedText($"%p learns that %p is their demon and that their fellow {(fellowMinions.Count > 1 ? "minions are" : "minion is")} %P.\n", minion, demon, fellowMinions, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText($"%p learns that %p is their demon.\n", minion, demon, StorytellerView);
            }
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            outputText.AppendFormattedText($"%p learns that %P {(minions.Count > 1 ? "are their minions" : "is their minion")}, and that the following characters are not in play: %C.\n", demon, minions, notInPlayCharacters, StorytellerView);
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                outputText.AppendFormattedText("%p learns that there are no outsiders in play.\n", godfather, StorytellerView);
                return;
            }
            outputText.AppendFormattedText("%p learns that the following outsiders are in play: %C\n", godfather, outsiders, StorytellerView);
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", librarian, playerA, playerB, character, StorytellerView);
        }

        public void NotifyInvestigator(Player investigator, Player playerA, Player playerB, Character character)
        {
            outputText.AppendFormattedText("%p learns that %p or %p is the %c.\n", investigator, playerA, playerB, character, StorytellerView);
        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            outputText.AppendFormattedText("%p learns that %p is a good player.\n", steward, goodPlayer, StorytellerView);
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendFormattedText($"%p learns that %b of their living neighbours (%p and %p) {(evilCount == 1 ? "is" : "are")} evil.\n", empath, evilCount, neighbourA, neighbourB, StorytellerView);
        }

        public void ChoiceFromImp(Player imp, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to kill %p.\n", imp, target, StorytellerView);
        }

        public void ChoiceFromAssassin(Player assassin, Player? target)
        {
            if (target == null)
            {
                outputText.AppendFormattedText("%p is not using their ability tonight.\n", assassin, StorytellerView);
            }
            else
            {
                outputText.AppendFormattedText("%p has chosen to kill %p.\n", assassin, target, StorytellerView);
            }
        }

        public void ChoiceFromGodfather(Player godfather, Player target)
        {
            outputText.AppendFormattedText("%p has chosen to kill %p.\n", godfather, target, StorytellerView);
        }


        public void ChoiceFromRavenkeeper(Player ravenkeeper, Player target, Character character)
        {
            outputText.AppendFormattedText("%p chooses %p and learns that they are the %c.\n", ravenkeeper, target, character, StorytellerView);
        }

        private const bool StorytellerView = true;
    }
}
