namespace Clocktower.Game
{
    public class TokensOnPlayer
    {
        public Character? Character => tokens.Select(pair => TokenToCharacter(pair.token)).FirstOrDefault(character => character != null);

        public bool DrunkOrPoisoned => HasDrunkOrPoisonToken() || HasToken(Token.IsTheMarionette) || NoDashiiPoisoned(out _);

        public TokensOnPlayer(Grimoire grimoire, Player player)
        {
            this.grimoire = grimoire;
            this.player = player;
        }

        public override string ToString()
        {
            var allTokens = new List<(Token token, Player player)>(tokens);
            if (NoDashiiPoisoned(out var noDashii) && noDashii != null)
            {
                allTokens.Add((Token.NoDashiiPoisoned, noDashii));
            }

            return string.Join(", ", allTokens.Order()
                                              .Select(pair => TokenPairToText(pair.token, pair.player))
                                              .Where(text => !string.IsNullOrEmpty(text)));
        }

        public void Add(Token token, Player assigningPlayer)
        {
            tokens.Add((token, assigningPlayer));
        }

        public void Remove(Token token)
        {
            tokens.RemoveAll(pair => pair.token == token);
        }

        public void Remove(Token token, Player assigningPlayer)
        {
            tokens.RemoveAll(pair => pair.token == token && pair.player == assigningPlayer);
        }

        public void ClearTokensForEndOfDay()
        {
            tokens.RemoveAll(pair => IsTokenThatExpiresAtEndOfDay(pair.token));
        }

        public void ClearTokensForEndOfNight()
        {
            tokens.RemoveAll(pair => IsTokenThatExpiresAtEndOfNight(pair.token));

            if (tokens.Any(pair => pair.token == Token.JugglerBeforeFirstDay))
            {
                tokens.RemoveAll(pair => pair.token == Token.JugglerBeforeFirstDay);
                tokens.Add((Token.JugglerFirstDay, player));
            }
        }

        public void ClearTokensOnPlayerDeath(Player playerWhoHasDied)
        {
            tokens.RemoveAll(pair => pair.player == playerWhoHasDied && IsTokenThatExpiresOnPlayerDeath(pair.token));
        }

        public void ClearTokensForPlayer(Player assigningPlayer)
        {
            tokens.RemoveAll(pair => pair.player == assigningPlayer);
        }

        public bool HasToken(Token token)
        {
            return tokens.Any(pair => pair.token == token);
        }

        public bool HasTokenForPlayer(Token token, Player assigningPlayer)
        {
            return tokens.Contains((token, assigningPlayer));
        }

        public int CountTokensForPlayer(Token token, Player assigningPlayer)
        {
            return tokens.Count(pair => pair.token == token && pair.player == assigningPlayer);
        }

        public Player GetAssigningPlayerForToken(Token token)
        {
            return tokens.FirstOrDefault(pair => pair.token == token).player;
        }

        /// <summary>
        /// Returns true if the player has this token and the assigning player is not drunk or poisoned.
        /// </summary>
        public bool HasHealthyToken(Token token)
        {
            return tokens.Any(pair => pair.token == token && !pair.player.DrunkOrPoisoned);
        }

        private static Character? TokenToCharacter(Token token)
        {
            return token switch
            {
                Token.IsTheDrunk => (Character?)Game.Character.Drunk,
                Token.IsTheMarionette => (Character?)Game.Character.Marionette,
                Token.IsTheCannibalPhilosopher => (Character?)Game.Character.Cannibal,
                Token.IsThePhilosopher or Token.IsTheBadPhilosopher => (Character?)Game.Character.Philosopher,
                _ => null,
            };
        }

        private static bool IsDrunkOrPoisonedToken(Token token)
        {
            return token == Token.IsTheDrunk
                || token == Token.SweetheartDrunk
                || token == Token.PhilosopherDrunk
                || token == Token.PoisonedByPoisoner
                || token == Token.CannibalPoisoned
                || token == Token.CannibalDrunk;
        }

        private static bool IsTokenThatExpiresAtEndOfDay(Token token)
        {
            return token == Token.PoisonedByPoisoner
                || token == Token.CursedByWitch
                || token == Token.ProtectedByDevilsAdvocate
                || token == Token.ProtectedByMonk
                || token == Token.ChosenByButler;
        }

        private static bool IsTokenThatExpiresAtEndOfNight(Token token)
        {
            return token == Token.Executed
                || token == Token.PhilosopherUsedAbilityTonight
                || token == Token.JuggledCorrectly
                || token == Token.CannibalFirstNightWithAbility;
        }

        private static bool IsTokenThatExpiresOnPlayerDeath(Token token)
        {   // These are generally only tokens that have an active effect on the game and so need to be removed immediately to stop them having their effect.
            return token == Token.PhilosopherDrunk
                || token == Token.NoDashiiPoisoned
                || token == Token.PoisonedByPoisoner
                || token == Token.CursedByWitch
                || token == Token.PickedByDevilsAdvocate
                || token == Token.ProtectedByDevilsAdvocate
                || token == Token.ProtectedByMonk
                || token == Token.ChosenByButler
                || token == Token.JugglerBeforeFirstDay
                || token == Token.JugglerFirstDay
                || token == Token.CannibalEaten
                || token == Token.CannibalPoisoned
                || token == Token.CannibalDrunk;
        }

        private string TokenPairToText(Token token, Player assigningPlayer)
        {
            if (assigningPlayer == player)
            {
                return TokenToText(token);
            }
            return $"{TokenToText(token)} ({assigningPlayer.Name})";
        }

        private static string TokenToText(Token token)
        {
            return token switch
            {
                Token.IsTheDrunk => "is the Drunk",
                Token.IsTheMarionette => "is the Marionette",
                Token.IsThePhilosopher => "is the Philosopher",
                Token.IsTheCannibalPhilosopher => "is the (Cannibal) Philosopher",
                Token.IsTheBadPhilosopher => "is the Philosopher (drunk/posioned when used)",
                Token.UsedOncePerGameAbility => "has used their once-per-game ability",
                Token.SweetheartDrunk => "Sweetheart drunk",
                Token.PhilosopherDrunk => "Philosopher drunk",
                Token.NoDashiiPoisoned => "poisoned by the No Dashii",
                Token.PoisonedByPoisoner => "poisoned by the Poisoner",
                Token.CannibalPoisoned => "poisoned from a bad meal",
                Token.CursedByWitch => "cursed by the Witch",
                Token.ProtectedByDevilsAdvocate => "protected by the Devil's Advocate",
                Token.FortuneTellerRedHerring => "Fortune Teller red herring",
                Token.GodfatherKillsTonight => "Godfather kills tonight",
                Token.ProtectedByMonk => "protected by the Monk",
                Token.ChosenByButler => "chosen by the Butler",
                Token.WasherwomanPing => "seen by the Washerwoman",
                Token.WasherwomanWrong => "incorrectly seen by the Washerwoman",
                Token.LibrarianPing => "seen by the Librarian",
                Token.LibrarianWrong => "incorrectly seen by the Librarian",
                Token.InvestigatorPing => "seen by the Investigator",
                Token.InvestigatorWrong => "incorrectly seen by the Investigator",
                Token.StewardPing => "seen by the Steward",
                Token.NoblePing => "seen by the Noble",
                Token.JuggledCorrectly => "juggled correctly by the Juggler",
                Token.CannibalEaten => "has ability as Cannibal",
                _ => string.Empty,  // All other tokens shouldn't be shown on the Grimoire - they're an implementation detail.
            };
        }

        private bool HasDrunkOrPoisonToken()
        {
            return tokens.Any(pair => IsDrunkOrPoisonedToken(pair.token) && (pair.player == player || !pair.player.DrunkOrPoisoned));
        }

        private bool NoDashiiPoisoned(out Player? noDashii)
        {
            // Are we a Townsfolk neighbour of a No Dashii (ignoring intervening non-Townsfolk)?

            noDashii = null;

            if (player.CharacterType != CharacterType.Townsfolk)
            {
                return false;
            }

            if (player.Character == Game.Character.Soldier && !HasDrunkOrPoisonToken())
            {
                return false;
            }

            if (HasHealthyToken(Token.ProtectedByMonk))
            {
                return false;
            }

            var players = grimoire.Players.ToList();
            if (!players.Any(player => player.RealCharacter == Game.Character.No_Dashii && !player.DrunkOrPoisoned))
            {
                return false;
            }

            // To determine this we go clockwise and counterclockwise from our seat until we encounter a Townsfolk or a healthy No Dashii.
            int myIndex = players.IndexOf(player);
            // Clockwise check.
            for (int step = 1; step < players.Count; step++)
            {
                var clockwisePlayer = players[(myIndex + step) % players.Count];
                if (clockwisePlayer.CharacterType == CharacterType.Townsfolk)
                {
                    break;
                }
                if (clockwisePlayer.RealCharacter == Game.Character.No_Dashii && !clockwisePlayer.DrunkOrPoisoned)
                {
                    noDashii = clockwisePlayer;
                    return true;
                }
            }
            // Counterclockwise check.
            for (int step = 1; step < players.Count; step++)
            {
                var counterclockwisePlayer = players[(myIndex + players.Count - step) % players.Count];
                if (counterclockwisePlayer.CharacterType == CharacterType.Townsfolk)
                {
                    break;
                }
                if (counterclockwisePlayer.RealCharacter == Game.Character.No_Dashii && !counterclockwisePlayer.DrunkOrPoisoned)
                {
                    noDashii = counterclockwisePlayer;
                    return true;
                }
            }
            // Townsfolk buffer between us and the No Dashii, so not poisoned by the No Dashii.
            return false;
        }

        private readonly Grimoire grimoire;
        private readonly Player player;
        private readonly List<(Token token, Player player)> tokens = new();  // Each token together with the player that token belongs to.
    }
}
