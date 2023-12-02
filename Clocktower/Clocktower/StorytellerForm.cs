using Clocktower.Game;
using System.Numerics;

namespace Clocktower
{
    public partial class StorytellerForm : Form
    {
        public StorytellerForm()
        {
            InitializeComponent();
        }

        public void AssignCharacter(string name, Character character, Alignment alignment)
        {
            outputText.AppendBoldText(name);
            outputText.AppendText(" is the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.AlignmentToColor(alignment));
            outputText.AppendText(".\n");
        }

        public void AssignCharacter(string name, Character realCharacter, Alignment realAlignment,
                                                 Character believedCharacter, Alignment believedAlignment)
        {
            outputText.AppendBoldText(name);
            outputText.AppendText(" believes they are the ");
            outputText.AppendText(TextUtilities.CharacterToText(believedCharacter), TextUtilities.AlignmentToColor(believedAlignment));
            outputText.AppendText(" but they are actually the ");
            outputText.AppendText(TextUtilities.CharacterToText(realCharacter), TextUtilities.AlignmentToColor(realAlignment));
            outputText.AppendText(".\n");
        }

        public void Night(int nightNumber)
        {
            outputText.AppendBoldText($"\nNight {nightNumber}\n\n");
        }

        public void Day(int dayNumber)
        {
            outputText.AppendBoldText($"\nDay {dayNumber}\n\n");
        }

        public void MinionInformation(Player minion, Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            AppendPlayerText(minion);
            outputText.AppendText(" learns that ");
            AppendPlayerText(demon);
            outputText.AppendText(" is their demon");

            if (fellowMinions.Count == 1)
            {
                outputText.AppendText(" and that their fellow minion is ");
                AppendPlayerText(fellowMinions.First());
            }
            else if (fellowMinions.Count == 2)
            {
                outputText.AppendText(" and that their fellow minions are ");
                AppendPlayerText(fellowMinions.First());
                outputText.AppendText(" and ");
                AppendPlayerText(fellowMinions.Last());
            }

            outputText.AppendText(".\n");
        }

        public void DemonInformation(Player demon, IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            AppendPlayerText(demon);
            outputText.AppendText(" learns that ");
            if (minions.Count == 1)
            {
                AppendPlayerText(minions.First());
                outputText.AppendText(" is their minion");
            }
            else if (minions.Count == 2)
            {
                AppendPlayerText(minions.First());
                outputText.AppendText(" and ");
                AppendPlayerText(minions.Last());
                outputText.AppendText(" are their minions");
            }
            else if (minions.Count == 3)
            {
                AppendPlayerText(minions.First());
                outputText.AppendText(", ");
                AppendPlayerText(minions.Skip(1).First());
                outputText.AppendText(" and ");
                AppendPlayerText(minions.Last());
                outputText.AppendText(" are their minions");
            }
            else
            {
                throw new ArgumentException("There must be exactly 1-3 minions");
            }

            outputText.AppendText(", and that the following characters are not in play: ");
            AppendCharacters(notInPlayCharacters);
            outputText.AppendText(".\n");
        }

        public void NotifyGodfather(Player godfather, IReadOnlyCollection<Character> outsiders)
        {
            AppendPlayerText(godfather);
            if (outsiders.Count == 0)
            {
                outputText.AppendText(" learns that there are no outsiders in play.");
                return;
            }
            outputText.AppendText(" learns that the following outsiders are in play: ");
            AppendCharacters(outsiders);
            outputText.AppendText(".\n");

        }

        public void NotifySteward(Player steward, Player goodPlayer)
        {
            AppendPlayerText(steward);
            outputText.AppendText(" learns that ");
            AppendPlayerText(goodPlayer);
            outputText.AppendText(" is a good player.\n");
        }

        public void NotifyLibrarian(Player librarian, Player playerA, Player playerB, Character character)
        {
            AppendPlayerText(librarian);
            outputText.AppendText(" learns that ");
            AppendPlayerText(playerA);
            outputText.AppendText(" or ");
            AppendPlayerText(playerB);
            outputText.AppendText(" is the ");
            AppendCharacterText(character);
            outputText.AppendText(".\n");
        }

        public void NotifyEmpath(Player empath, Player neighbourA, Player neighbourB, int evilCount)
        {
            AppendPlayerText(empath);
            outputText.AppendText(" learns that ");
            outputText.AppendBoldText(evilCount.ToString());
            outputText.AppendText(" of their living neighbours (");
            AppendPlayerText(neighbourA);
            outputText.AppendText(" and ");
            AppendPlayerText(neighbourB);
            if (evilCount == 1)
            {
                outputText.AppendText(") is evil.\n");
            }
            else
            {
                outputText.AppendText(") are evil.\n");
            }
        }

        private void AppendPlayerText(Player player)
        {
            outputText.AppendBoldText(player.Name);
            if (player.Character.HasValue && player.RealCharacter.HasValue && player.Alignment.HasValue && player.RealAlignment.HasValue)
            {
                outputText.AppendText(" (");

                if (player.Character == player.RealCharacter)
                {
                    outputText.AppendText(TextUtilities.CharacterToText(player.Character.Value), TextUtilities.AlignmentToColor(player.RealAlignment.Value));
                }
                else
                {
                    outputText.AppendText(TextUtilities.CharacterToText(player.Character.Value), TextUtilities.AlignmentToColor(player.Alignment.Value));
                    outputText.AppendText("/");
                    outputText.AppendText(TextUtilities.CharacterToText(player.RealCharacter.Value), TextUtilities.AlignmentToColor(player.RealAlignment.Value));
                }

                outputText.AppendText(")");
            }
        }

        private void AppendCharacterText(Character character)
        {
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
        }

        private void AppendCharacters(IReadOnlyCollection<Character> characters)
        {
            bool first = true;
            foreach (var character in characters)
            {
                if (!first)
                {
                    outputText.AppendText(", ");
                }
                AppendCharacterText(character);
                first = false;
            }
        }

    }
}
