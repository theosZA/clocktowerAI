namespace Clocktower.Game
{
    public static class CharacterExtensions
    {
        public static CharacterType CharacterType(this Character character)
        {
            return (int)character switch
            {
                < 1000 => Game.CharacterType.Townsfolk,
                < 2000 => Game.CharacterType.Outsider,
                < 3000 => Game.CharacterType.Minion,
                _ => Game.CharacterType.Demon
            };
        }

        public static IEnumerable<Character> OfCharacterType(this IEnumerable<Character> characters, CharacterType characterType)
        {
            return characters.Where(character => character.CharacterType() == characterType);
        }

        public static Alignment Alignment(this Character character)
        {
            return (int)character switch
            {
                < 2000 => Game.Alignment.Good,
                _ => Game.Alignment.Evil
            };
        }

        public static IEnumerable<Character> OfAlignment(this IEnumerable<Character> characters, Alignment alignment)
        {
            return characters.Where(character => character.Alignment() == alignment);
        }
    }
}