using Clocktower.Agent;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public ClocktowerGame()
        {
            var storytellerForm = new StorytellerForm();
            storyteller = new HumanStoryteller(storytellerForm);
            storytellerForm.Show();

            var playerNames = new[] { "Alison", "Barry", "Casandra", "Donald", "Emma", "Franklin", "Georgina", "Harry" };
            var forms = playerNames.Select(name => new HumanAgentForm(name)).ToList();
            var agents = forms.Select(form => new HumanAgent(form)).ToList();
            players = playerNames.Zip(agents).Select(x => new Player(x.First, x.Second)).ToList();

            foreach (var form in forms)
            {
                form.Show();
            }

            players[0].AssignCharacter(Character.Steward, Alignment.Good);
            players[1].AssignCharacter(Character.Imp, Alignment.Evil);
            players[2].AssignCharacter(Character.Godfather, Alignment.Evil);
            players[3].AssignCharacter(Character.Recluse, Alignment.Good);
            players[4].AssignCharacter(Character.Slayer, Alignment.Good);
            players[5].AssignCharacter(Character.Empath, Alignment.Good);
            players[6].AssignCharacter(Character.Drunk, Alignment.Good,
                                       Character.Librarian, Alignment.Good);
            players[7].AssignCharacter(Character.Ravenkeeper, Alignment.Good);

            foreach (var player in players)
            {
                if (player.Character.HasValue && player.RealCharacter.HasValue && player.Alignment.HasValue && player.RealAlignment.HasValue)
                {
                    if (player.Character == player.RealCharacter && player.Alignment == player.RealAlignment)
                    {
                        storyteller.AssignCharacter(player.Name, player.Character.Value, player.Alignment.Value);
                    }
                    else
                    {
                        storyteller.AssignCharacter(player.Name, player.RealCharacter.Value, player.RealAlignment.Value,
                                                                 player.Character.Value, player.Alignment.Value);
                    }
                }
            }
        }

        private List<Player> players;
        private IStoryteller storyteller;
    }
}
