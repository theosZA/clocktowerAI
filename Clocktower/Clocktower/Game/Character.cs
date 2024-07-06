// To add a new character:
// 1. Add the character to this Character enum.
// 2. Add the character to a script (in the Scripts folder) for testing.
// 3. Add the character description to Characters.txt.
// 4. If relevant, add the character to the night order (NightOrder.txt and/or FirstNightOrder.txt) or day (Day.txt).
//    a. Create an event for the character's action. Add the implementation of IEvent to the Events folder.
//       This event will likely require adding one or more of the following:
//       - Storyteller requests - add a GetXXX method to StorytellerExtensions and a matching GetXXX method to IStoryteller and implementation.
//       - Player requests - add a RequestXXX method to AgentExtensions and a matching RequestXXX method to IAgent and implementations.
//       - Notifications - add a NotifyXXX method to IStoryteller, IAgent and/or IObserver (and implementations) as appropriate.
//       - Tokens - Add a new entry to the Token enum.
//                - Most tokens should be removed when the character dies; do that in the Grimoire.RemoveTokensForCharacter() method.
//                - For tokens that need to be cleared each night, do that in the StartNight event.
//    b. Add the event to the GameEventFactory.
// 5. Some characters will have abilities that can't be run on an event. You will need to add the functionality in the relevant place,
//    e.g. within the Nominations event if it's an ability based on nominations.

namespace Clocktower.Game
{
    public enum Character
    {
        // Townsfolk
        Steward = 0,
        Investigator,
        Librarian,
        Shugenja,
        Empath,
        Fortune_Teller,
        Undertaker,
        Monk,
        Fisherman,
        Slayer,
        Philosopher,
        Soldier,
        Ravenkeeper,
        Washerwoman,
        Virgin,
        Chef,
        Mayor,

        // Outsiders
        Tinker = 1000,
        Sweetheart,
        Recluse,
        Drunk,
        Saint,
        Butler,

        // Minions
        Godfather = 2000,
        Poisoner,
        Assassin,
        Scarlet_Woman,
        Baron,

        // Demons
        Imp = 3000
    }
}