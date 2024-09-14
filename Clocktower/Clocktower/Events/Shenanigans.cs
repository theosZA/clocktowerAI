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
        public Shenanigans(IStoryteller storyteller, Grimoire grimoire, Deaths deaths, IGameObserver observers, IReadOnlyCollection<Character> scriptCharacters, Random random, int dayNumber, bool afterNominations)
        {
            this.storyteller = storyteller;
            this.grimoire = grimoire;
            this.deaths = deaths;
            this.observers = observers;
            this.scriptCharacters = scriptCharacters;
            this.random = random;
            this.dayNumber = dayNumber;
            this.afterNominations = afterNominations;
        }

        public async Task RunEvent()
        {
            var players = grimoire.Players.Where(player => player.Alive).ToList();
            players.Shuffle(random);
            foreach (var player in players)
            {
                await RunShenanigansForPlayer(player);
                if (grimoire.Finished)
                {   // No more public actions needed if this player's public action ends the game.
                    return;
                }
            }
        }

        private async Task RunShenanigansForPlayer(Player player)
        {
            var options = BuildShenaniganOptions(player);
            if (!options.Any())
            {
                return;
            }

            var shenanigan = await player.Agent.PromptShenanigans(options, afterNominations, grimoire.PlayerToBeExecuted);

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
            else if (shenanigan is MinionGuessingDamselOption damselGuess && damselGuess.Target != null)
            {
                await HandleDamselGuess(player, damselGuess.Target);
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
                if (player.Character.CharacterType() == CharacterType.Minion && scriptCharacters.Contains(Character.Damsel))
                {   // Ideally we'd also exclude this option if the player knows that an unsuccesful guess has already been made,
                    // but that's hard to be certain of when a script might obfuscate who are minions, e.g. with a Marionette in play.
                    AddDamselOptions(options);
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
                if (scriptCharacters.Contains(Character.Damsel))
                {   // Strictly speaking dead minions should be allowed to guess the Damsel.
                    // For convenience though, we're only allowing public actions from living players.
                    AddDamselOptions(options);
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

        private void AddDamselOptions(IList<IOption> options)
        {
            options.Add(new MinionGuessingDamselOption(grimoire.Players));
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
            juggler.Tokens.Add(Token.JugglerHasJuggled, juggler);

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

        private async Task HandleDamselGuess(Player minion, Player damsel)
        {
            if (minion.CharacterType == CharacterType.Minion)
            { 
                if (damsel.HasHealthyAbility(Character.Damsel) && !damsel.Tokens.HasToken(Token.DamselGuessUsed))
                {
                    await observers.AnnounceDamselGuess(minion, damsel, success: true);
                    grimoire.EndGame(damsel.Alignment == Alignment.Good ? Alignment.Evil : Alignment.Good);
                    return;
                }

                var realDamsels = grimoire.Players.Where(player => player.Character == Character.Damsel);
                foreach (var realDamsel in realDamsels)
                {
                    if (!realDamsel.Tokens.HasToken(Token.DamselGuessUsed))
                    {
                        realDamsel.Tokens.Add(Token.DamselGuessUsed, minion);
                    }
                }
            }
            await observers.AnnounceDamselGuess(minion, damsel, success: false);
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
        private readonly bool afterNominations;
    }
}
