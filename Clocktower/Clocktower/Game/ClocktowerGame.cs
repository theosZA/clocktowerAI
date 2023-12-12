using Clocktower.Agent;
using Clocktower.Events;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Game
{
    /// <summary>
    /// An instance of a Blood on the Clocktower game.
    /// </summary>
    internal class ClocktowerGame
    {
        public bool Finished => GetWinner().HasValue;

        public ClocktowerGame(IGameSetup setup, Random random)
        {
            this.setup = setup;
            this.random = random;

            var playerNames = new[] { "Alison", "Bernard", "Christie", "David", "Eleanor", "Franklin", "Georgina", "Harry", "Ingrid", "Julian", "Katie", "Leonard", "Maddie", "Norm", "Olivia" }.Take(setup.PlayerCount);
            var agents = playerNames.Select(name => (IAgent)new HumanAgentForm(name, setup.Script, random)).ToList();

            storyteller = new StorytellerForm(random);
            observers = ProxyCollection<IGameObserver>.CreateProxy(agents.Select(agent => agent.Observer).Append(storyteller.Observer));
            grimoire = new Grimoire(agents, setup.Characters);

            StartGame(agents);
        }

        public Alignment? GetWinner()
        {
            // The game is over if there are no living demons...
            if (!grimoire.Players.Any(player => player.Alive && player.CharacterType == CharacterType.Demon))
            {
                return Alignment.Good;
            }

            // ...or there are fewer than 3 players alive.
            if (grimoire.Players.Count(player => player.Alive) < 3)
            {
                return Alignment.Evil;
            }

            return null;
        }

        public void AnnounceWinner()
        {
            var winner = GetWinner();
            if (winner.HasValue)
            {
                observers.AnnounceWinner(winner.Value,
                                         grimoire.Players.Where(player => player.Alignment == winner.Value).ToList(),
                                         grimoire.Players.Where(player => player.Alignment != winner.Value).ToList());
            }
        }

        public async Task RunNightAndDay()
        {
            if (dayNumber == 0)
            {
                await RunEventSequence(setup.BuildAdditionalSetupEvents(storyteller, grimoire));
            }

            ++dayNumber;

            observers.Night(dayNumber);
            if (dayNumber == 1)
            {
                await RunFirstNight();
            }
            else
            {
                await RunNight();
            }

            observers.Day(dayNumber);
            await RunDay();
        }

        private async Task RunFirstNight()
        {
            await RunEventSequence(new IGameEvent[]
            {
                new AssignFortuneTellerRedHerring(storyteller, grimoire),
                new ChoiceFromPhilosopher(storyteller, grimoire, setup.Script),
                new MinionInformation(storyteller, grimoire),
                new DemonInformation(storyteller, grimoire, setup.Script, random),
                new ChoiceFromPoisoner(storyteller, grimoire),
                new NotifyGodfather(storyteller, grimoire),
                new NotifyLibrarian(storyteller, grimoire, setup.Script, random),
                new NotifyInvestigator(storyteller, grimoire, setup.Script, random),
                new NotifyEmpath(storyteller, grimoire),
                new ChoiceFromFortuneTeller(storyteller, grimoire),
                new NotifySteward(storyteller, grimoire),
                new NotifyShugenja(storyteller, grimoire)
            });
        }

        private async Task RunNight()
        {
            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.PoisonedByPoisoner);
                player.Tokens.Remove(Token.ProtectedByMonk);
                player.Tokens.Remove(Token.AlreadyClaimedSlayer);   // We allow players to claim Slayer once each day to allow for Philosopher into Slayer.
            }

            await RunEventSequence(new IGameEvent[]
            {
                new ChoiceFromPhilosopher(storyteller, grimoire, setup.Script),
                new ChoiceFromPoisoner(storyteller, grimoire),
                new ChoiceFromMonk(storyteller, grimoire),
                // Scarlet Woman - this is their theoretical place in the night order, but they actually become the demon immediately
                new ChoiceFromImp(storyteller, grimoire),
                new ChoiceFromAssassin(storyteller, grimoire),
                new ChoiceFromGodfather(storyteller, grimoire),
                new SweetheartDrunk(storyteller, grimoire),
                new TinkerOption(storyteller, grimoire, observers, duringDay: false),
                new NotifyPhilosopherStartKnowing(storyteller, grimoire, setup.Script, random),
                new ChoiceFromRavenkeeper(storyteller, grimoire, setup.Script),
                new NotifyEmpath(storyteller, grimoire),
                new ChoiceFromFortuneTeller(storyteller, grimoire),
                new NotifyUndertaker(storyteller, grimoire, setup.Script)
            });

            // Clear expired tokens.
            foreach (var player in grimoire.Players)
            {
                player.Tokens.Remove(Token.Executed);
                player.Tokens.Remove(Token.PhilosopherUsedAbilityTonight);
            }
        }

        private async Task RunDay()
        {
            var nominations = new Nominations(storyteller, grimoire, observers, random);

            await RunEventSequence(new IGameEvent[]
            {
                new AnnounceNightKills(grimoire, observers, dayNumber),
                new TinkerOption(storyteller, grimoire, observers, duringDay: true),
                new PublicStatements(grimoire, observers, random, morning: true),
                new FishermanAdvice(storyteller, grimoire),
                new SlayerShot(storyteller, grimoire, observers, random),
                // TBD Conversations during the day. Add the following options in once we support conversations (otherwise they're duplicated without need).
                // --private conversations--
                // Fisherman
                // Slayer
                // --public conversations--
                // Tinker
                new RollCall(grimoire, observers),
                new PublicStatements(grimoire, observers, random, morning: false),
                // Fisherman
                // Slayer
                nominations,
                new SlayerShot(storyteller, grimoire, observers, random) { Nominations = nominations },
                new FishermanAdvice(storyteller, grimoire) { Nominations = nominations },
                new TinkerOption(storyteller, grimoire, observers, duringDay: true)
            });

            if (!Finished)
            {
                if (nominations.PlayerToBeExecuted == null)
                {
                    observers.DayEndsWithNoExecution();
                }
                else
                {
                    bool playerDies = nominations.PlayerToBeExecuted.Alive;
                    observers.PlayerIsExecuted(nominations.PlayerToBeExecuted, playerDies);
                    if (playerDies)
                    {
                        new Kills(storyteller, grimoire).Execute(nominations.PlayerToBeExecuted);
                    }
                }
            }
        }

        private async Task RunEventSequence(IEnumerable<IGameEvent> events)
        {
            foreach (var gameEvent in events)
            {
                await gameEvent.RunEvent();
                if (Finished)
                {
                    return;
                }
            }
        }

        private void StartGame(IEnumerable<IAgent> agents)
        {
            storyteller.Start();
            foreach (var agent in agents)
            {
                agent.StartGame();
            }

            grimoire.AssignCharacters(storyteller);
        }

        private readonly IGameSetup setup;
        private readonly Random random;

        private readonly IStoryteller storyteller;
        private readonly IGameObserver observers;
        private readonly Grimoire grimoire;
    
        private int dayNumber = 0;
    }
}
