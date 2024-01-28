namespace Clocktower
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T RandomPick<T>(this IList<T> list, Random random)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException("Can't pick from an empty list", nameof(list));
            }
            return list[random.Next(list.Count)];
        }

        public static IEnumerable<T> RandomPickN<T>(this IList<T> list, int count, Random random)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (random.Next(list.Count - i) < count)
                {
                    yield return list[i];
                    count--;
                    if (count == 0)
                    {
                        yield break;
                    }
                }
            }
        }
    }
}
