namespace Clocktower.Game
{
    public enum Token
    {
        // Character modifying tokens - these should be shown first to anyone viewing the Grimoire.
        IsTheDrunk = 1,
        IsThePhilosopher,
        IsTheBadPhilosopher,
        // Normal tokens
        UsedOncePerGameAbility = 1000,
        SweetheartDrunk,
        PhilosopherDrunk,
        NoDashiiPoisoned,
        PoisonedByPoisoner,
        CursedByWitch,
        FortuneTellerRedHerring,
        GodfatherKillsTonight,
        ProtectedByMonk,
        ChosenByButler,
        WasherwomanPing,
        WasherwomanWrong,
        LibrarianPing,
        LibrarianWrong,
        InvestigatorPing,
        InvestigatorWrong,
        StewardPing,
        JuggledCorrectly,
        // Special marker (not actually represented by a token in the real game).
        DiedAtNight = 3000,
        // Not real tokens, just used here for tracking stuff needed for this implementation.
        Executed = 4000,
        PhilosopherUsedAbilityTonight,
        NeverBluffingShenanigans,    // a player may opt out of future "Shenanigans" phases unless they gain an ability which can be used then
        JugglerBeforeFirstDay,
        JugglerFirstDay
    }
}