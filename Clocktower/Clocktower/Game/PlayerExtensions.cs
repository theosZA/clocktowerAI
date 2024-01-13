namespace Clocktower.Game
{
    public static class PlayerExtensions
    {
        public static int Alive(this IEnumerable<Player> players)
        {
            return players.Count(player => player.Alive);
        }

        public static IEnumerable<Player> WithCharacter(this IEnumerable<Player> players, Character character) 
        {
            return players.Where(player => player.Character == character);
        }

        public static IEnumerable<Player> WithToken(this IEnumerable<Player> players, Token token) 
        {
            return players.Where(player => player.Tokens.Contains(token));
        }

        public static IEnumerable<Player> WithoutToken(this IEnumerable<Player> players, Token token)
        {
            return players.Where(player => !player.Tokens.Contains(token));
        }
    }
}
