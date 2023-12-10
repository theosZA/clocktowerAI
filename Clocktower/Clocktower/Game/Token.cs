namespace Clocktower.Game
{
    public enum Token
    {
        KilledByDemon,
        DiedAtNight,
        IsTheDrunk,
        SweetheartDrunk,
        PoisonedByPoisoner,
        FortuneTellerRedHerring,
        UsedOncePerGameAbility,
        GodfatherKillsTonight,
        Executed,
        ProtectedByMonk,
        AlreadyClaimedSlayer    // once a player has claimed Slayer and taken a shot that day, we no longer allow them to claim a second time (regardless of whether they're the real Slayer or not)
    }
}