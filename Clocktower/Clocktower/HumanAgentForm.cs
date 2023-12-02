﻿using Clocktower.Game;

namespace Clocktower
{
    public partial class HumanAgentForm : Form
    {
        public HumanAgentForm(string playerName)
        {
            InitializeComponent();

            Text = playerName;
        }

        public void AssignCharacter(Character character, Alignment alignment)
        {
            outputText.AppendText("You are the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.AlignmentToColor(alignment));
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

        public void MinionInformation(Player demon, IReadOnlyCollection<Player> fellowMinions)
        {
            outputText.AppendText("As a minion, you learn that ");
            outputText.AppendBoldText(demon.Name, Color.Red);
            outputText.AppendText(" is your demon");
            
            if (fellowMinions.Count == 1)
            {
                outputText.AppendText(" and your fellow minion is ");
                outputText.AppendBoldText(fellowMinions.First().Name, Color.Red);
            }
            else if (fellowMinions.Count == 2)
            {
                outputText.AppendText(" and your fellow minions are ");
                outputText.AppendBoldText(fellowMinions.First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(fellowMinions.Last().Name, Color.Red);
            }

            outputText.AppendText(".\n");
        }

        public void DemonInformation(IReadOnlyCollection<Player> minions, IReadOnlyCollection<Character> notInPlayCharacters)
        {
            outputText.AppendText("As a demon, you learn that ");
            if (minions.Count == 1)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(" is your minion");
            }
            else if (minions.Count == 2)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(minions.Last().Name, Color.Red);
                outputText.AppendText(" are your minions");
            }
            else if (minions.Count == 3)
            {
                outputText.AppendBoldText(minions.First().Name, Color.Red);
                outputText.AppendText(", ");
                outputText.AppendBoldText(minions.Skip(1).First().Name, Color.Red);
                outputText.AppendText(" and ");
                outputText.AppendBoldText(minions.Last().Name, Color.Red);
                outputText.AppendText(" are your minions");
            }
            else
            {
                throw new ArgumentException("There must be exactly 1-3 minions");
            }

            outputText.AppendText(", and that the following characters are not in play: ");
            outputText.AppendText(TextUtilities.CharacterToText(notInPlayCharacters.First()), TextUtilities.CharacterToColor(notInPlayCharacters.First()));
            foreach (var character in notInPlayCharacters.Skip(1))
            {
                outputText.AppendText(", ");
                outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
            }
            outputText.AppendText(".\n");
        }

        public void NotifyGodfather(IReadOnlyCollection<Character> outsiders)
        {
            if (outsiders.Count == 0)
            {
                outputText.AppendText("You learn that there are no outsiders in play.");
                return;
            }
            outputText.AppendText("You learn that the following outsiders are in play: ");
            AppendCharacterList(outsiders);
            outputText.AppendText(".\n");
        }

        public void NotifySteward(Player goodPlayer)
        {
            outputText.AppendText("You learn that ");
            outputText.AppendBoldText(goodPlayer.Name);
            outputText.AppendText(" is a good player.\n");
        }

        public void NotifyLibrarian(Player playerA, Player playerB, Character character)
        {
            outputText.AppendText("You learn that either ");
            outputText.AppendBoldText(playerA.Name);
            outputText.AppendText(" or ");
            outputText.AppendBoldText(playerB.Name);
            outputText.AppendText(" is the ");
            outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
            outputText.AppendText(".\n");
        }

        public void NotifyEmpath(Player neighbourA, Player neighbourB, int evilCount)
        {
            outputText.AppendText("You learn that ");
            outputText.AppendBoldText(evilCount.ToString());
            outputText.AppendText(" of your living neighbours (");
            outputText.AppendBoldText(neighbourA.Name);
            outputText.AppendText(" and ");
            outputText.AppendBoldText(neighbourB.Name);
            if (evilCount == 1)
            {
                outputText.AppendText(") is evil.\n");
            }
            else
            {
                outputText.AppendText(") are evil.\n");
            }
        }

        private void AppendCharacterList(IReadOnlyCollection<Character> characters)
        {
            bool first = true;
            foreach (var character in characters)
            {
                if (!first)
                {
                    outputText.AppendText(", ");
                }
                outputText.AppendText(TextUtilities.CharacterToText(character), TextUtilities.CharacterToColor(character));
                first = false;
            }
        }
    }
}
