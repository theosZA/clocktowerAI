namespace Clocktower.Game
{
    public class TokensOnPlayer
    {
        public Character? Character => tokens.Select(pair => TokenToCharacter(pair.token)).FirstOrDefault(character => character != null);

        public bool DrunkOrPoisoned => tokens.Any(pair => IsDrunkOrPoisonedToken(pair.token) && (pair.player == player || !pair.player.DrunkOrPoisoned));

        public TokensOnPlayer(Player player)
        {
            this.player = player;
        }

        public override string ToString()
        {
            return string.Join(", ", tokens.Order()
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

        public void ClearTokensForEndOfDay()
        {
            tokens.RemoveAll(pair => IsTokenThatExpiresAtEndOfDay(pair.token));
        }

        public void ClearTokensForEndOfNight()
        {
            tokens.RemoveAll(pair => IsTokenThatExpiresAtEndOfNight(pair.token));
        }

        public bool HasToken(Token token)
        {
            return tokens.Any(pair => pair.token == token);
        }

        public bool HasTokenForPlayer(Token token, Player assigningPlayer)
        {
            return tokens.Contains((token, assigningPlayer));
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
                Token.IsThePhilosopher or Token.IsTheBadPhilosopher => (Character?)Game.Character.Philosopher,
                _ => null,
            };
        }

        private static bool IsDrunkOrPoisonedToken(Token token)
        {
            return token == Token.IsTheDrunk
                || token == Token.SweetheartDrunk
                || token == Token.PhilosopherDrunk
                || token == Token.PoisonedByPoisoner;
        }

        private static bool IsTokenThatExpiresAtEndOfDay(Token token)
        {
            return token == Token.PoisonedByPoisoner
                || token == Token.ProtectedByMonk
                || token == Token.ChosenByButler
                || token == Token.AlreadyClaimedSlayer; // We allow players to claim Slayer once each day to allow for Philosopher into Slayer.
        }

        private static bool IsTokenThatExpiresAtEndOfNight(Token token)
        {
            return token == Token.Executed
                || token == Token.PhilosopherUsedAbilityTonight;
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
                Token.IsThePhilosopher => "is the Philosopher",
                Token.IsTheBadPhilosopher => "is the Philosopher (drunk/posioned when used)",
                Token.UsedOncePerGameAbility => "has used their once-per-game ability",
                Token.SweetheartDrunk => "Sweetheart drunk",
                Token.PhilosopherDrunk => "Philosopher drunk",
                Token.PoisonedByPoisoner => "poisoned by Poisoner",
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
                _ => string.Empty,  // All other tokens shouldn't be shown on the Grimoire - they're an implementation detail.
            };
        }

        private readonly Player player;
        private readonly List<(Token token, Player player)> tokens = new();  // Each token together with the player that token belongs to.
    }
}
