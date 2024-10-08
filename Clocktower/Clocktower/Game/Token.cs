﻿namespace Clocktower.Game
{
    public enum Token
    {
        // Character modifying tokens - these should be shown first to anyone viewing the Grimoire.
        IsTheDrunk = 1,
        IsTheMarionette,
        IsThePhilosopher,
        IsTheCannibalPhilosopher,   // needed to disambiguate from the Philo-Cannibal
        IsTheBadPhilosopher,
        // Normal tokens
        UsedOncePerGameAbility = 1000,
        MinionKilledByVigormortis,
        SweetheartDrunk,
        PhilosopherDrunk,
        NoDashiiPoisoned,
        PoisonedByPukka,
        PoisonedByVigormortis,
        PoisonedByPoisoner,
        PoisonedByWidow,
        CannibalPoisoned,
        SnakeCharmerPoisoned,
        CursedByWitch,
        ProtectedByDevilsAdvocate,
        FortuneTellerRedHerring,
        GodfatherKillsTonight,
        ProtectedByMonk,
        KilledByLycanthrope,
        ChosenByButler,
        WasherwomanPing,
        WasherwomanWrong,
        LibrarianPing,
        LibrarianWrong,
        InvestigatorPing,
        InvestigatorWrong,
        StewardPing,
        NoblePing,
        BalloonistPing,
        BountyHunterPing,
        JuggledCorrectly,
        CannibalEaten,
        DamselGuessUsed,
        DamselJinxPoisoned,
        // Special marker (not actually represented by a token in the real game).
        DiedAtNight = 3000,
        // Not real tokens, just used here for tracking stuff needed for this implementation.
        Executed = 4000,
        PhilosopherUsedAbilityTonight,
        PickedByDevilsAdvocate,     // tracks separately from the Devil's Advocate protection because they may be poisoned/drunk but are still restricted in their choice the next day
        NeverBluffingShenanigans,   // a player may opt out of future "Shenanigans" phases unless they gain an ability which can be used then
        JugglerBeforeFirstDay,
        JugglerFirstDay,
        JugglerHasJuggled,
        CannibalFirstNightWithAbility,
        CannibalDrunk   // when the Cannibal has specifically gained the Drunk's ability
    }
}