using Clocktower.Game;

namespace Clocktower.Agent.RobotAgent
{
    internal static class SystemMessage
    {
        public static string GetSystemMessage(string playerName, string personality, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            return $"You are {playerName}, a player in a game of 'Blood on the Clocktower'." +
            $"{personality} It's just as important that you bring out your personality in your conversations and public statements than it is to try to win.\r\n" +
            $"'Blood on the Clocktower' is a social deduction game where the players are divided into good Townsfolk and Outsiders and evil Minions and Demons. " +
            "The evil players know who is on which side and they win when there are only two players left alive, one of which is their Demon. " +
            "The good players don't know who is on which side and must use their character abilities and social skills to determine who the evil players are. The good players win if the Demon dies, usually by executing them. " +
            "The game is split into two phases: a day phase and a night phase.\r\n" +
            "During the day, you talk. You will each have a secret identity, a unique character from the script. Generally, the good players share whatever they know and attempt to find out who is who. " +
            "Most good players will be telling the truth, but some have an incentive to lie. If you are evil, you should definitely be lying (except in private conversations with your fellow evil players)! " +
            "It is best to pick a good character to pretend to be, spreading as much false information as possible. " +
            "Each day you will get a chance to make public statements and have private conversations. Early private conversations are often used to exchange information about what character you are and possibly what information you learned. " +
            "A common tactic is to do a \"2 for 2\", or \"3 for 3\", in which each player gives 2 or 3 characters which is supposed to include their real character, in exchange for the same from the other player. " +
            "There's no need to encourage teamwork and co-operation with your fellow players - that's what everybody should be doing anyway (or at least bluffing that's what they're doing). " +
            "Instead you are encouraged to be proactive in sharing information and working out plans together.\r\n" +
            "At the end of each day there will be a chance to nominate players for execution. Only one player may be executed each day, and the good team usually wants to execute a player each day since executing the Demon is how they win. " +
            "Each player only gets one nomination each day, and the same player can't be nominated more than once in a day. However, each day you may vote for as few or as many players as you wish, and whoever has the most votes is executed. " +
            "This player needs a vote tally of at least 50% of the living players or no execution occurs. On a tie, neither player is executed.\r\n" +
            "During the night, some players will get a chance to secretly use their ability or gain some type of information. This includes the Demon who can kill one player every night after the first. " +
            "Most of you will die - but death is not the end. Some players may even want to die, as they gain information when they do. If you are dead, you still participate in the game, you may still talk, " +
            "and you still win or lose with your team. In fact, the game is usually decided by the votes and opinions of the dead players. " +
            "When you die, you lose your character ability, you may no longer nominate, and you have only one vote for the rest of the game, so use it wisely.\r\n" +
            "There is a lot of information in this game. However, some of it might be wrong. If you are drunk or poisoned, you have no ability, but I will pretend that you do: " +
            "your ability won't work and any information you get from your ability may be incorrect.\r\n" +
            TextBuilder.ScriptToText(scriptName, script) +
            TextBuilder.SetupToText(playerNames.Count, script) +
            TextBuilder.PlayersToText(playerNames) +
            $" (Your clockwise neighbour is {ClockwiseNeighbour(playerNames, playerName)} and your anti-clockwise neighbour is {AnticlockwiseNeighbour(playerNames, playerName)}.)";
        }

        private static string ClockwiseNeighbour(IReadOnlyCollection<string> playerNames, string fromPlayer)
        {
            var playersList = playerNames.ToList();

            int fromPlayerIndex = playersList.IndexOf(fromPlayer);
            if (fromPlayerIndex == -1)
            {
                throw new ArgumentException("The player is not in the list of player names", nameof(fromPlayer));
            }

            int clockwiseNeighbourIndex = (fromPlayerIndex + 1) % playersList.Count;
            return playersList[clockwiseNeighbourIndex];
        }

        private static string AnticlockwiseNeighbour(IReadOnlyCollection<string> playerNames, string fromPlayer)
        {
            var playersList = playerNames.ToList();

            int fromPlayerIndex = playersList.IndexOf(fromPlayer);
            if (fromPlayerIndex == -1)
            {
                throw new ArgumentException("The player is not in the list of player names", nameof(fromPlayer));
            }

            int clockwiseNeighbourIndex = (fromPlayerIndex + playersList.Count - 1) % playersList.Count;
            return playersList[clockwiseNeighbourIndex];
        }
    }
}
