using Clocktower.Agent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public bool Finished => false;

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

            dayNumber = 1;
            phase = Phase.Night;
        }

        public void RunPhase()
        {
            switch (phase)
            {
                case Phase.Night:
                    storyteller.Night(dayNumber);
                    foreach (var player in players)
                    {
                        player.Agent.Night(dayNumber);
                    }
                    if (dayNumber == 1)
                    {
                        RunFirstNight();
                    }
                    else
                    {
                        RunNight();
                    }
                    break;

                case Phase.Morning:
                    storyteller.Day(dayNumber);
                    foreach (var player in players)
                    {
                        player.Agent.Day(dayNumber);
                    }
                    // TBD
                    break;

                case Phase.Day:
                case Phase.Evening:
                    // TBD
                    break;

                default:
                    throw new InvalidEnumArgumentException(nameof(phase));
            }

            AdvancePhase();
        }

        private void RunFirstNight()
        {
            var demon = GetDemon();
            var minions = GetMinions().ToList();

            // Philosopher

            // Minion info
            foreach (var minion in minions)
            {
                minion.Agent.MinionInformation(demon, minions.Except(new[] { minion }).ToList());
                storyteller.MinionInformation(minion, demon, minions.Except(new[] { minion }).ToList());
            }

            // Demon info
            var notInPlayCharacters = new[] { Character.Fortune_Teller, Character.Philosopher, Character.Soldier };  // hardcoded list for now
            demon.Agent.DemonInformation(minions, notInPlayCharacters);
            storyteller.DemonInformation(demon, minions, notInPlayCharacters);

            // Poisoner

            // Godfather
            var godfather = GetPlayer(Character.Godfather);
            if (godfather != null)
            {
                var outsiders = GetOutsiders().ToList();
                godfather.Agent.NotifyGodfather(outsiders);
                storyteller.NotifyGodfather(godfather, outsiders);
            }

            // Librarian
            var librarian = GetPlayer(Character.Librarian);
            if (librarian != null)
            {
                // For now we give them a hardcoded pair of players and the outsider 'Drunk'.
                var librarianTargetA = IsDrunk(librarian) ? GetRequiredPlayer(Character.Imp) : GetRequiredRealPlayer(Character.Drunk);
                var librarianTargetB = GetRequiredPlayer(Character.Empath);
                librarian.Agent.NotifyLibrarian(librarianTargetA, librarianTargetB, Character.Drunk);
                storyteller.NotifyLibrarian(librarian, librarianTargetA, librarianTargetB, Character.Drunk);
            }

            // Investigator

            // Empath
            var empath = GetPlayer(Character.Empath);
            if (empath != null)
            {
                RunEmpath(empath);
            }

            // Fortune Teller

            // Steward
            var steward = GetPlayer(Character.Steward);
            if (steward != null)
            {
                // For now we give them a hardcoded player.
                var stewardTarget = IsDrunk(steward) ? GetRequiredPlayer(Character.Imp) : GetRequiredPlayer(Character.Ravenkeeper);
                steward.Agent.NotifySteward(stewardTarget);
                storyteller.NotifySteward(steward, stewardTarget);
            }

            // Shugenja

        }

        private void RunNight()
        {
            // Philosopher

            // Poisoner

            // Monk

            // Scarlet Woman

            // Imp

            // Assassin

            // Godfather

            // Sweetheart

            // Tinker

            // Ravenkeeper

            // Empath
            var empath = GetPlayer(Character.Empath);
            if (empath != null)
            {
                RunEmpath(empath);
            }

            // Fortune Teller

            // Undertaker
        }

        private void AdvancePhase()
        {
            switch (phase)
            {
                case Phase.Night:
                    phase = Phase.Morning;
                    break;

                case Phase.Morning:
                    phase = Phase.Day;
                    break;

                case Phase.Day:
                    phase = Phase.Evening;
                    break;

                case Phase.Evening:
                    phase = Phase.Night;
                    ++dayNumber;
                    break;
            }
        }

        private void RunEmpath(Player empath)
        {
            var livingNeighbours = GetLivingNeighbours(empath);

            int evilCount = 0;
            if (GetEmpathAlignment(livingNeighbours.Item1) == Alignment.Evil)
            {
                ++evilCount;
            }
            if (GetEmpathAlignment(livingNeighbours.Item2) == Alignment.Evil)
            {
                ++evilCount;
            }

            empath.Agent.NotifyEmpath(livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
            storyteller.NotifyEmpath(empath, livingNeighbours.Item1, livingNeighbours.Item2, evilCount);
        }

        private (Player, Player) GetLivingNeighbours(Player player)
        {
            int myIndex = players.IndexOf(player);
            return (GetPreviousLivingPlayer(myIndex), GetNextLivingPlayer(myIndex));
        }

        private Player GetNextLivingPlayer(int startIndex)
        {
            int index = (startIndex + 1) % players.Count;
            while (!players[index].Alive)
            {
                index = (index + 1) % players.Count;
            }
            return players[index];
        }

        private Player GetPreviousLivingPlayer(int startIndex)
        {
            int index = (startIndex + players.Count - 1) % players.Count;
            while (!players[index].Alive)
            {
                index = (index + players.Count - 1) % players.Count;
            }
            return players[index];
        }

        private Player? GetPlayer(Character believedCharacter)
        {
            return players.FirstOrDefault(player => player.Character == believedCharacter);
        }

        private Player GetRequiredPlayer(Character believedCharacter)
        {
            return players.First(player => player.Character == believedCharacter);
        }

        private Player GetRequiredRealPlayer(Character believedCharacter)
        {
            return players.First(player => player.RealCharacter == believedCharacter);
        }

        private Player GetDemon()
        {
            return players.First(player => player.Character.HasValue && CharacterTypeFromCharacter(player.Character.Value) == CharacterType.Demon);
        }

        private IEnumerable<Player> GetMinions()
        {
            return players.Where(player => player.Character.HasValue && CharacterTypeFromCharacter(player.Character.Value) == CharacterType.Minion);
        }

        private IEnumerable<Character> GetOutsiders()
        {
            return players.Where(player => player.RealCharacter.HasValue && CharacterTypeFromCharacter(player.RealCharacter.Value) == CharacterType.Outsider)
                          .Select(player => player.RealCharacter ?? (Character)(-1));
        }

        private static Alignment GetEmpathAlignment(Player player)
        {
            if (player.RealCharacter.HasValue && player.RealCharacter == Character.Recluse)
            {
                return Alignment.Evil;  // Note that this doesn't have to be evil, but then requires storyteller input.
            }
            return player.RealAlignment ?? Alignment.Good;
        }

        private static bool IsDrunk(Player player)
        {
            return player != null && player.RealCharacter.HasValue && player.RealCharacter == Character.Drunk;
        }

        private static CharacterType CharacterTypeFromCharacter(Character character)
        {
            if ((int)character < 1000)
            {
                return CharacterType.Townsfolk;
            }
            if ((int)character < 2000)
            {
                return CharacterType.Outsider;
            }
            if ((int)character < 3000)
            {
                return CharacterType.Demon;
            }
            return CharacterType.Minion;
        }

        private List<Player> players;
        private IStoryteller storyteller;

        private int dayNumber;
        private Phase phase;
    }
}
