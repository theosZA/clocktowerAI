using Clocktower.Agent.Observer;
using Clocktower.EventScripts;
using Clocktower.Game;
using Clocktower.Setup;
using Clocktower.Storyteller;
using Clocktower.Triggers;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Clocktower.Events
{
    internal class GameEventFactory
    {
        public GameEventFactory(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, IGameSetup setup, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = new Deaths(storyteller, grimoire, setup.Script);
            this.observers = observers;
            this.setup = setup;
            this.random = random;
        }

        public IGameEvent BuildEvent(string name, int dayNumber, bool duringDay)
        {
            switch (name)
            {
                // Set-up events
                case "AssignMarionette":            return new AssignMarionette(storyteller, grimoire);
                case "AssignDrunk":                 return new AssignDrunk(storyteller, setup, grimoire);
                case "AssignEvilTownsfolk":         return new AssignEvilTownsfolk(storyteller, grimoire);
                case "FortuneTellerRedHerring":     return new AssignFortuneTellerRedHerring(storyteller, grimoire);
                case "DemonInformation":            return new DemonInformation(storyteller, grimoire, setup.Script, random);
                case "MinionInformation":           return new MinionInformation(storyteller, grimoire, setup.Script, random);

                // Storyteller announcements / game progression
                case "StartDay":                    return new StartDay(grimoire, observers, dayNumber);
                case "EndDay":                      return new EndDay(storyteller, grimoire, deaths, observers);
                case "StartNight":                  return new StartNight(grimoire, observers, dayNumber);
                case "EndNight":                    return new EndNight(grimoire);

                // Day activities
                case "Nominations":                 return new Nominations(storyteller, grimoire, deaths, observers, setup.Script, random);
                case "PrivateChats":                return new PrivateChats(storyteller, grimoire, observers, random, dayNumber, setup.PlayerCount);
                case "MorningPublicStatements":     return new PublicStatements(grimoire, observers, random, morning: true);
                case "EveningPublicStatements":     return new PublicStatements(grimoire, observers, random, morning: false);
                case "FinalPublicStatements":       return new PublicStatements(grimoire, observers, random, morning: false) { OnlyAlivePlayers = true };
                case "RollCall":                    return new RollCall(grimoire, observers);
                case "Shenanigans":                 return new Shenanigans(storyteller, grimoire, deaths, observers, setup.Script, random, dayNumber, afterNominations: false);
                case "LastChanceShenanigans":       return new Shenanigans(storyteller, grimoire, deaths, observers, setup.Script, random, dayNumber, afterNominations: true);

                // Townsfolk
                case "Chef":                        return new NotifyChef(storyteller, grimoire);
                case "Empath":                      return new NotifyEmpath(storyteller, grimoire);
                case "Fisherman":                   return new FishermanAdvice(storyteller, grimoire);
                case "FortuneTeller":               return new ChoiceFromFortuneTeller(storyteller, grimoire);
                case "Investigator":                return new NotifyInvestigator(storyteller, grimoire, setup.Script, random);
                case "Librarian":                   return new NotifyLibrarian(storyteller, grimoire, setup.Script, random);
                case "Monk":                        return new ChoiceFromMonk(storyteller, grimoire);
                case "Philosopher":                 return new ChoiceFromPhilosopher(storyteller, grimoire, setup.Script, firstNight: dayNumber == 1);
                case "PhilosopherInformation":      return new NotifyPhilosopherStartKnowing(storyteller, grimoire, setup.Script, random);
                case "CannibalInformation":         return new NotifyCannibalStartKnowing(storyteller, grimoire, setup.Script, random);
                case "Shugenja":                    return new NotifyShugenja(storyteller, grimoire);
                case "Steward":                     return new NotifySteward(storyteller, grimoire);
                case "Noble":                       return new NotifyNoble(storyteller, grimoire, random);
                case "Undertaker":                  return new NotifyUndertaker(storyteller, grimoire, setup.Script);
                case "Washerwoman":                 return new NotifyWasherwoman(storyteller, grimoire, setup.Script, random);
                case "Juggler":                     return new NotifyJuggler(storyteller, grimoire);
                case "Balloonist":                  return new NotifyBalloonist(storyteller, grimoire);
                case "Nightwatchman":               return new ChoiceFromNightwatchman(storyteller, grimoire);
                case "Oracle":                      return new NotifyOracle(storyteller, grimoire);
                case "Huntsman":                    return new ChoiceFromHuntsman(storyteller, grimoire, setup.Script, random, firstNight: dayNumber == 1);
                case "SnakeCharmer":                return new ChoiceFromSnakeCharmer(storyteller, grimoire);
                case "BountyHunter":                return new NotifyBountyHunter(storyteller, grimoire);

                // Outsiders
                case "Tinker":                      return new TinkerOption(storyteller, grimoire, deaths, observers, duringDay);
                case "Butler":                      return new ChoiceFromButler(storyteller, grimoire);
                case "Acrobat":                     return new CheckAcrobat(storyteller, grimoire, deaths);
                case "Ogre":                        return new ChoiceFromOgre(storyteller, grimoire);

                // Minions
                case "Assassin":                    return new ChoiceFromAssassin(storyteller, grimoire, deaths);
                case "Godfather":                   return new ChoiceFromGodfather(storyteller, grimoire, deaths);
                case "GodfatherInformation":        return new NotifyGodfather(storyteller, grimoire);
                case "Poisoner":                    return new ChoiceFromPoisoner(storyteller, grimoire);
                case "Witch":                       return new ChoiceFromWitch(storyteller, grimoire);
                case "Spy":                         return new ShowGrimoireToSpy(storyteller, grimoire);
                case "DevilsAdvocate":              return new ChoiceFromDevilsAdvocate(storyteller, grimoire);
                case "Widow":                       return new ChoiceFromWidow(storyteller, grimoire);

                // Demons
                case "Imp":                         return new ChoiceFromDemon(Character.Imp, storyteller, grimoire, deaths);
                case "NoDashii":                    return new ChoiceFromDemon(Character.No_Dashii, storyteller, grimoire, deaths);
                case "Ojo":                         return new ChoiceFromOjo(storyteller, grimoire, deaths, setup.Script);
                case "Kazali":                      return new ChoiceFromDemon(Character.Kazali, storyteller, grimoire, deaths);
                case "KazaliMinions":               return new SelectionOfKazaliMinions(storyteller, grimoire, setup.Script);

                default: throw new Exception($"Unknown event name \"{name}\"");
            }
        }

        public IGameEvent BuildNightEvents(int nightNumber)
        {
            var eventScript = (nightNumber == 1) ? firstNightOrder : nightOrder;
            return BuildSequenceEvent(eventScript.EventNodes, nightNumber, duringDay: false);
        }

        public IGameEvent BuildDayEvents(int dayNumber)
        {
            return BuildSequenceEvent(day.EventNodes, dayNumber, duringDay: true);
        }

        private IGameEvent BuildSequenceEvent(IEnumerable<IEventScriptNode> eventScriptNodes, int dayNumber, bool duringDay)
        {
            var events = eventScriptNodes.Select(eventScriptNode => BuildEventFromNode(eventScriptNode, dayNumber, duringDay));
            return new SequenceEvent(grimoire, events);
        }

        private IGameEvent BuildConditionalEvent(string expression, IEventScriptNode childNode, int dayNumber, bool duringDay)
        {
            var globals = new EventScriptGlobals(setup, grimoire);
            var scriptOptions = ScriptOptions.Default.AddReferences(GetType().Assembly)
                                                     .AddImports("System")
                                                     .AddImports("Clocktower.Game.PlayerExtensions");
            var condition = async () => await CSharpScript.EvaluateAsync<bool>(expression, globals: globals, globalsType: typeof(EventScriptGlobals), options: scriptOptions);
            var wrappedEvent = BuildEventFromNode(childNode, dayNumber, duringDay);
            return new ConditionalEvent(wrappedEvent, condition);
        }

        private IGameEvent BuildEventFromNode(IEventScriptNode eventScriptNode, int dayNumber, bool duringDay)
        {
            switch (eventScriptNode)
            {
                case EventNode eventNode:
                    return BuildEvent(eventNode.Name, dayNumber, duringDay);

                case SequenceNode sequenceNode:
                    return BuildSequenceEvent(sequenceNode.Children, dayNumber, duringDay);

                case ConditionalNode conditionalNode:
                    return BuildConditionalEvent(conditionalNode.Expression, conditionalNode.Child, dayNumber, duringDay);

                default:
                    throw new ArgumentException("Unrecognized event script node type", nameof(eventScriptNode));
            }
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
        private readonly IGameObserver observers;
        private readonly IGameSetup setup;
        private readonly Random random;

        private readonly EventScript firstNightOrder = new("EventScripts\\FirstNightOrder.txt");
        private readonly EventScript nightOrder = new("EventScripts\\NightOrder.txt");
        private readonly EventScript day = new("EventScripts\\Day.txt");
    }
}
