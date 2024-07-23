using Clocktower.Game;
using Clocktower.Observer;
using Clocktower.Options;
using Clocktower.Storyteller;

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
        public Shenanigans(IStoryteller storyteller, Grimoire grimoire, IGameObserver observers, IReadOnlyCollection<Character> scriptCharacters, Random random, int dayNumber)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
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
            else if (shenanigan is SlayerShotOption slayerShot)
            {
                await HandleSlayerClaim(player, slayerShot);
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
                if (player.Character == Character.Slayer && !player.Tokens.HasToken(Token.UsedOncePerGameAbility))
                {
                    AddSlayerOptions(options);
                }
                if (player.Character == Character.Juggler && player.Tokens.HasToken(Token.JugglerFirstDay))
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

        private void AddSlayerOptions(List<IOption> options)
        {
            options.AddRange(grimoire.Players.Select(player => new SlayerShotOption(player)));
        }

        private void AddJugglerOptions(IList<IOption> options)
        {
            // Juggles are a special case where we don't generate all the options - there would be way too many of them!
            // Instead we pass the Juggle option to the agent with an empty list that the agent can populate.
            options.Add(new JugglerOption(grimoire.Players, scriptCharacters));
        }

        private async Task HandleSlayerClaim(Player slayer, SlayerShotOption slayerShot)
        {
            bool success = await DoesKillTarget(slayer, slayerShot.Target);
            await observers.AnnounceSlayerShot(slayer, slayerShot.Target, success);

            if (slayer.Character == Character.Slayer)
            {
                slayer.Tokens.Add(Token.UsedOncePerGameAbility, slayer);
            }

            if (success)
            {
                await new Kills(storyteller, grimoire).DayKill(slayerShot.Target, slayer);
            }
        }

        private async Task HandleJugglerClaim(Player juggler, JugglerOption juggles)
        {
            await observers.AnnounceJuggles(juggler, juggles.Juggles);

            if (juggler.Character != Character.Juggler)
            {
                return;
            }
            
            if (!juggler.Tokens.HasToken(Token.JugglerFirstDay))
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
            if (target.Character == Character.Tinker && !target.DrunkOrPoisoned)
            {   // The Tinker can die at any time, so doesn't even need a real Slayer to shoot them.
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target);
            }
            if (purportedSlayer.Character != Character.Slayer)
            {
                return false;
            }
            if (purportedSlayer.Tokens.HasToken(Token.UsedOncePerGameAbility))
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
                return await storyteller.ShouldKillWithSlayer(purportedSlayer, target);
            }
            return true;
        }

        private readonly IStoryteller storyteller;
        private readonly Grimoire grimoire;
        private readonly IGameObserver observers;
        private readonly IReadOnlyCollection<Character> scriptCharacters;
        private readonly Random random;
        private readonly int dayNumber;
    }
}
