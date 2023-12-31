﻿namespace Clocktower.Game
{
    public enum Token
    {
        DiedAtNight,
        IsTheDrunk,
        IsThePhilosopher,
        IsTheBadPhilosopher,
        SweetheartDrunk,
        PhilosopherDrunk,
        PoisonedByPoisoner,
        FortuneTellerRedHerring,
        PhilosopherFortuneTellerRedHerring,
        PhilosopherUsedAbilityTonight,
        UsedOncePerGameAbility,
        GodfatherKillsTonight,
        Executed,
        ProtectedByMonk,
        AlreadyClaimedSlayer,   // once a player has claimed Slayer and taken a shot that day, we no longer allow them to claim a second time (regardless of whether they're the real Slayer or not)
        NeverBluffingSlayer     // a player may opt out of bluffing Slayer in which case they are never again prompted for a Slayer shot
    }
}