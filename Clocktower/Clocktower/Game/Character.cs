// To add a new character:
// 1. Add the character to this Character enum.
// 2. Add the character to a script (in the Scripts folder) for testing. Alternatively it can be tested with the Whale Bucket script.
// 3. Add the character description to Characters.txt.
// 4. If relevant, add the character to the night order (NightOrder.txt and/or FirstNightOrder.txt) or day (Day.txt).
//    a. Create an event for the character's action. Add the implementation of IEvent to the Events folder.
//       This event will likely require adding one or more of the following:
//       - Storyteller requests - add a GetXXX method to StorytellerExtensions and a matching GetXXX method to IStoryteller and implementation.
//       - Player requests - add a RequestXXX method to AgentExtensions and a matching RequestXXX method to IAgent and implementations.
//       - Notifications - add a NotifyXXX method to IStoryteller, IAgent and/or IObserver (and implementations) as appropriate.
//       - Tokens - Add a new entry to the Token enum.
//                - Check methods in the TokensOnPlayer class for token behaviour, especially when they should be removed and what should be shown to the Spy.
//       - When we should run an ability is controlled in Player.ShouldRunAbility(); e.g. once-per-game abilities can be checked here if they've been used or not.
//    b. Add the event to the GameEventFactory.
// 5. For characters that interact with deaths or executions, add an implementation of IDeathTrigger in the Triggers folder, and add it to the Deaths class.
// 6. Some characters will have abilities that can't be run on an event or death trigger. You will need to add the functionality in the relevant place,
//    e.g. within the Nominations event if it's an ability based on nominations, or in the Player class if it's a character that can misregister.
// 7. Add scenario tests for the character. These tests may include the following:
//    - Regular postive and negative test cases.
//    - Ensure once-per-game abilities can only be used once.
//    - Cases for when the character is really the Drunk or otherwise drunk.
//    - Cases for then the character has been poisoned.
//    - Cases for when the character dies and their effect on the game should end.
//    - Cases for when this character ability is chosen by the Philosopher.
//    - Cases for when this character ability is gained by the Cannibal or poisoned Cannibal.
// 8. Add jinxes. These include adding the descriptions to Jinxes.txt, scenario tests to cover the jinxes, and implementations for the jinxes.
// 9. Note any house rules that are implemented for the character in HouseRules.txt.

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
        Juggler,
        Pacifist,
        Noble,
        Cannibal,
        Balloonist,
        Nightwatchman,
        Oracle,

        // Outsiders
        Tinker = 1000,
        Sweetheart,
        Recluse,
        Drunk,
        Saint,
        Butler,
        Snitch,
        Acrobat,
        Damsel,

        // Minions
        Godfather = 2000,
        Poisoner,
        Assassin,
        Scarlet_Woman,
        Baron,
        Spy,
        Witch,
        Organ_Grinder,
        Devils_Advocate,
        Marionette,
        Widow,

        // Demons
        Imp = 3000,
        Ojo,
        No_Dashii,
        Kazali
    }
}