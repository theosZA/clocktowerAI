using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class GameEventFactory
    {
        public GameEventFactory(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, IGameSetup setup, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.setup = setup;
            this.random = random;
        }

        public IEnumerable<IGameEvent> BuildNightEvents(int nightNumber)
        {
            if (nightNumber == 1)
            {
                return setup.BuildAdditionalSetupEvents(storyteller, grimoire).Concat(BuildFirstNightEvents());
            }
            return BuildSubsequentNightEvents(nightNumber);
        }

        public IEnumerable<IGameEvent> BuildDayEvents(int dayNumber)
        {
            yield return new StartDay(observers, dayNumber);
            yield return new AnnounceNightKills(grimoire, observers, dayNumber);
            yield return new TinkerOption(storyteller, grimoire, observers, duringDay: true);

            yield return new ConditionalEvent(new PublicStatements(grimoire, observers, random, morning: true), () => grimoire.Players.Count(player => player.Alive) > 4);
            yield return new ConditionalEvent(new FishermanAdvice(storyteller, grimoire), () => grimoire.Players.Count(player => player.Alive) > 4);
            yield return new ConditionalEvent(new SlayerShot(storyteller, grimoire, observers, random), () => grimoire.Players.Count(player => player.Alive) > 4);

            var privateChats = new RepeatedEvent(new PrivateChats(storyteller, grimoire, observers, random),
                                                 i => i < HowManyPrivateChats(dayNumber, setup.PlayerCount, grimoire.Players.Count(player => player.Alive)));
            yield return privateChats;
            yield return new ConditionalEvent(new FishermanAdvice(storyteller, grimoire), () => privateChats.IterationsRun > 0);
            yield return new ConditionalEvent(new SlayerShot(storyteller, grimoire, observers, random), () => privateChats.IterationsRun > 0);
            yield return new ConditionalEvent(new TinkerOption(storyteller, grimoire, observers, duringDay: true), () => privateChats.IterationsRun > 0);

            yield return new ConditionalEvent(new RollCall(grimoire, observers), () => grimoire.Players.Count(player => player.Alive) <= 4);
            var eveningPublicStatements = new PublicStatements(grimoire, observers, random, morning: false);
            yield return eveningPublicStatements;
            yield return new ConditionalEvent(new PublicStatements(grimoire, observers, random, morning: false) { OnlyAlivePlayers = true },
                                              () => eveningPublicStatements.StatementsCount > 0 && grimoire.Players.Count(player => player.Alive) == 3);
            yield return new ConditionalEvent(new FishermanAdvice(storyteller, grimoire), () => eveningPublicStatements.StatementsCount > 0);
            yield return new ConditionalEvent(new SlayerShot(storyteller, grimoire, observers, random), () => eveningPublicStatements.StatementsCount > 0);
            yield return new ConditionalEvent(new TinkerOption(storyteller, grimoire, observers, duringDay: true), () => eveningPublicStatements.StatementsCount > 0);

            var nominations = new Nominations(storyteller, grimoire, observers, random);
            yield return nominations;
            yield return new FishermanAdvice(storyteller, grimoire) { Nominations = nominations };
            yield return new SlayerShot(storyteller, grimoire, observers, random) { Nominations = nominations };
            yield return new TinkerOption(storyteller, grimoire, observers, duringDay: true);
            yield return new EndDay(storyteller, grimoire, observers, nominations);
        }

        private IEnumerable<IGameEvent> BuildFirstNightEvents()
        {
            yield return new StartNight(grimoire, observers, nightNumber: 1);
            yield return new AssignFortuneTellerRedHerring(storyteller, grimoire);
            yield return new ChoiceFromPhilosopher(storyteller, grimoire, setup.Script);
            yield return new MinionInformation(storyteller, grimoire);
            yield return new DemonInformation(storyteller, grimoire, setup.Script, random);
            yield return new ChoiceFromPoisoner(storyteller, grimoire);
            yield return new NotifyGodfather(storyteller, grimoire);
            yield return new NotifyLibrarian(storyteller, grimoire, setup.Script, random);
            yield return new NotifyInvestigator(storyteller, grimoire, setup.Script, random);
            yield return new NotifyEmpath(storyteller, grimoire);
            yield return new ChoiceFromFortuneTeller(storyteller, grimoire);
            yield return new NotifySteward(storyteller, grimoire);
            yield return new NotifyShugenja(storyteller, grimoire);
            yield return new EndNight(grimoire);
        }

        private IEnumerable<IGameEvent> BuildSubsequentNightEvents(int nightNumber)
        {
            yield return new StartNight(grimoire, observers, nightNumber);
            yield return new ChoiceFromPhilosopher(storyteller, grimoire, setup.Script);
            yield return new ChoiceFromPoisoner(storyteller, grimoire);
            yield return new ChoiceFromMonk(storyteller, grimoire);
            // Scarlet Woman - this is their theoretical place in the night order, but they actually become the demon immediately
            yield return new ChoiceFromImp(storyteller, grimoire);
            yield return new ChoiceFromAssassin(storyteller, grimoire);
            yield return new ChoiceFromGodfather(storyteller, grimoire);
            yield return new SweetheartDrunk(storyteller, grimoire);
            yield return new TinkerOption(storyteller, grimoire, observers, duringDay: false);
            yield return new NotifyPhilosopherStartKnowing(storyteller, grimoire, setup.Script, random);
            yield return new ChoiceFromRavenkeeper(storyteller, grimoire, setup.Script);
            yield return new NotifyEmpath(storyteller, grimoire);
            yield return new ChoiceFromFortuneTeller(storyteller, grimoire);
            yield return new NotifyUndertaker(storyteller, grimoire, setup.Script);
            yield return new EndNight(grimoire);
        }

        private static int HowManyPrivateChats(int dayNumber, int playerCount, int alivePlayersLeft)
        {
            if (alivePlayersLeft <= 4)
            {
                return 0;
            }

            int firstDayPrivateChats = playerCount switch
            {
                <= 7 => 2,
                <= 9 => 3,
                <= 11 => 4,
                <= 13 => 5,
                _ => 6
            };
            // Reduce the number of chats by 1 per day to a minimum of 1.
            int currentDayPrivateChats = firstDayPrivateChats - (dayNumber - 1);
            return currentDayPrivateChats < 1 ? 1 : currentDayPrivateChats;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly IGameSetup setup;
        private readonly Random random;
    }
}
