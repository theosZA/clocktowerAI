using Clocktower.Agent.Observer;
using Clocktower.Game;
using Clocktower.Options;
using Clocktower.Storyteller;
using Clocktower.Triggers;

namespace Clocktower.Events
{
    /// <summary>
    /// The "Shenanigans" phase is an unofficial time during the day for players to publicly use their day abilities.
    /// Examples include the once-per-game ability of the Slayer, the each day ability of the Psychopath, or the ability for a Minion to guess the Damsel.
    /// Importantly you do not actually need to be the matching character in order to claim to use the ability, it just needs to be on the script.
    /// A player always has the option to opt out of any future Shenanigans.
    /// </summary>
    internal class Shenanigans : IGameEvent
    {
        public Shenanigans(IStoryteller storyteller, Grimoire grimoire, Deaths deaths, IGameObserver observers, IReadOnlyCollection<Character> scriptCharacters, Random random, int dayNumber)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
            this.observers = observers;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
            this.dayNumber = dayNumber;
        }

        public async Task RunEvent()
        {
            var players = grimoire.Players.Where(player => player.Alive).ToList();
            players.Shuffle(random);
            foreach (var player in players)
            {
                await RunShenanigansForPlayer(player);
            }
        }

        private async Task RunShenanigansForPlayer(Player player)
        {
            var options = BuildShenaniganOptions(player);
            if (!options.Any())
            {
                return;
            }

            var shenanigan = await player.Agent.PromptShenanigans(options);

            if (shenanigan is AlwaysPassOption)
            {
                player.Tokens.Add(Token.NeverBluffingShenanigans, player);
            }
            else if (shenanigan is SlayerShotOption slayerShot && slayerShot.Target != null)
            {
                await HandleSlayerClaim(player, slayerShot.Target);
            }
            else if (shenanigan is JugglerOption juggles)
            {
                await HandleJugglerClaim(player, juggles);
            }
        }

        private IReadOnlyCollection<IOption> BuildShenaniganOptions(Player player)
        {
            var options = new List<IOption>();

            if (player.Tokens.HasToken(Token.NeverBluffingShenanigans))
            {   // Only include options for legitimate claims.
                if (player.ShouldRunAbility(Character.Slayer))
                {
                    AddSlayerOptions(options);
                }
                if (player.ShouldRunAbility(Character.Juggler))
                {
                    AddJugglerOptions(options);
                }
            }
            else
            {   // Include options for all claims on the script
                if (scriptCharacters.Contains(Character.Slayer))
                {
                    AddSlayerOptions(options);
                }
                if (scriptCharacters.Contains(Character.Juggler))
                {
                    // Juggles are only allowed on the first day unless the script has a way of making
                    // new jugglers on subsequent days.
                    if (dayNumber == 1 || scriptCharacters.Contains(Character.Philosopher))
                    {
                        AddJugglerOptions(options);
                    }
                }
            }

            if (options.Count > 0)
            {
                options.Insert(0, new PassOption());
                if (!player.Tokens.HasToken(Token.NeverBluffingShenanigans))
                {
                    options.Insert(0, new AlwaysPassOption());
                }
            }

            return options;
        }

        private void AddSlayerOptions(IList<IOption> options)
        {
            options.Add(new SlayerShotOption(grimoire.Players));
        }

        private void AddJugglerOptions(IList<IOption> options)
        {
            options.Add(new JugglerOption(grimoire.Players, scriptCharacters));
        }

        private async Task HandleSlayerClaim(Player slayer, Player target)
        {
            bool success = await DoesKillTarget(slayer, target);
            await observers.AnnounceSlayerShot(slayer, target, success);

            if (slayer.Character == Character.Slayer)
            {
                slayer.Tokens.Add(Token.UsedOncePerGameAbility, slayer);
            }

            if (success)
            {
                await deaths.DayKill(target, slayer);
            }
        }

        private async Task HandleJugglerClaim(Player juggler, JugglerOption juggles)
        {
            await observers.AnnounceJuggles(juggler, juggles.Juggles);

            if (!juggler.ShouldRunAbility(Character.Juggler))
            {
                return;
            }
            
            foreach (var juggle in juggles.Juggles)
            {
                if (juggle.player.RealCharacter == juggle.character)
                {
                    juggle.player.Tokens.Add(Token.JuggledCorrectly, juggler);
                }
                else if (juggle.player.CanRegisterAs(juggle.character))
                {
                    if (await storyteller.ShouldRegisterForJuggle(juggler, juggle.player, juggle.character))
                    {
                        juggle.player.Tokens.Add(Token.JuggledCorrectly, juggler);
                    }
                }
            }
        }

        private async Task<bool> DoesKillTarget(Player purportedSlayer, Player target)
        {
            if (target.HasHealthyAbility(Character.Tinker))
            {   // The Tinker can die at any time, so doesn't even need a real Slayer to shoot them.
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target);
            }
            if (!purportedSlayer.HasHealthyAbility(Character.Slayer))
            {
                return false;
            }
            if (!target.CanRegisterAsDemon)
            {
                return false;
            }
            if (target.CharacterType != CharacterType.Demon)
            {   // Target isn't a demon, but can register as a demon. Need storyteller's choice.
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target);
            }
            return true;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly Deaths deaths;
        private readonly IGameObserver observers;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
        private readonly int dayNumber;
    }
}
