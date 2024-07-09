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
        PoisonedByPoisoner,
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
        // Philosopher-version of normal tokens (for cases where both the original character and Philosopher-version of the character need to track their tokens separately).
        PhilosopherFortuneTellerRedHerring = 2000,
        ChosenByPhiloButler,
        // Special marker (not actually represented by a token in the real game).
        DiedAtNight = 3000,
        // Not real tokens, just used here for tracking stuff needed for this implementation.
        Executed = 4000,
        PhilosopherUsedAbilityTonight,
        AlreadyClaimedSlayer,   // once a player has claimed Slayer and taken a shot that day, we no longer allow them to claim a second time (regardless of whether they're the real Slayer or not)
        NeverBluffingSlayer     // a player may opt out of bluffing Slayer in which case they are never again prompted for a Slayer shot
    }
}