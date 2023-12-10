using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using Clocktower.Storyteller;

namespace Clocktower.Events
{
    internal class SlayerShot : IGameEvent
    {
        /// <summary>
        /// Current day nominations - only set if we want to restrict the Slayer shot option to the player who is about to be executed.
        /// </summary>
        public Nominations? Nominations { get; set; }

        public SlayerShot(IStoryteller storyteller, Grimoire grimoire, ObserverCollection observers, Random random)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.observers = observers;
            this.random = random;
        }

        /// <summary>
        /// While the Slayer can take their shot at any time, to save from checking in with all players so frequently, we should only check
        /// at the start of the day (after night deaths have been announced), at the start of public discussion before nominations,
        /// after public discussion before nominations, and after nominations before ending the day.
        /// </summary>
        public async Task RunEvent()
        {
            var players = GetPlayersWhoCanStillClaimSlayer();
            players.Shuffle(random);
            foreach (var player in players)
            {
                var target = await GetTarget(player);
                if (target != null) 
                {
                    await ShootTarget(player, target);
                }
            }
        }

        private List<Player> GetPlayersWhoCanStillClaimSlayer()
        {
            if (Nominations != null)
            {   // We only allow the player about to be executed to still claim Slayer here.
                if (Nominations.PlayerToBeExecuted != null && CanPlayerClaimSlayer(Nominations.PlayerToBeExecuted))
                {
                    return new List<Player> { Nominations.PlayerToBeExecuted };
                }
                return new List<Player>();
            }
            return grimoire.Players.Where(CanPlayerClaimSlayer).ToList();
        }

        private static bool CanPlayerClaimSlayer(Player player)
        {
            return player.Alive && !player.Tokens.Contains(Token.AlreadyClaimedSlayer);
        }

        private IReadOnlyCollection<IOption> GetSlayerShotOptions(Player purportedSlayer)
        {
            if (purportedSlayer.Tokens.Contains(Token.AlreadyClaimedSlayer))
            {
                return Array.Empty<IOption>();
            }

            // While a Slayer can theoretically target any player, there is no benefit to targeting themselves or any dead player,
            // so they are excluded as possible targets.
            return grimoire.Players.Where(player => player.Alive && player != purportedSlayer)
                                   .ToSlayerShotOptions(bluff: purportedSlayer.Character != Character.Slayer);
        }

        private async Task ShootTarget(Player purportedSlayer, Player target)
        {
            purportedSlayer.Tokens.Add(Token.AlreadyClaimedSlayer);
            if (purportedSlayer.Character == Character.Slayer)
            {
                purportedSlayer.Tokens.Add(Token.UsedOncePerGameAbility);
            }

            bool success = await DoesKillTarget(purportedSlayer, target);
            observers.AnnounceSlayerShot(purportedSlayer, target, success);
            if (success)
            {
                new Kills(storyteller, grimoire).DayKill(target);
            }
        }

        private async Task<Player?> GetTarget(Player purportedSlayer)
        {
            var options = GetSlayerShotOptions(purportedSlayer).Prepend(new PassOption()).ToList();
            return (await purportedSlayer.Agent.PromptSlayerShot(options)).GetSlayerTargetOptional();
        }

        private async Task<bool> DoesKillTarget(Player purportedSlayer, Player target)
        {
            if (target.Character == Character.Tinker)
            {   // The Tinker can die at any time, so doesn't even need a real Slayer to shoot them.
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target, OptionsBuilder.YesOrNo) is YesOption;
            }
            if (purportedSlayer.Character != Character.Slayer)
            {
                return false;
            }
            if (purportedSlayer.DrunkOrPoisoned)
            {
                return false;
            }
            if (!target.CanRegisterAsDemon)
            {
                return false;
            }
            if (target.CharacterType != CharacterType.Demon)
            {   // Target isn't a demon, but can register as a demon. Need storyteller's choice.
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target, OptionsBuilder.YesOrNo) is YesOption;
            }
            return true;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly ObserverCollection observers;
        private readonly Random random;
    }
}
