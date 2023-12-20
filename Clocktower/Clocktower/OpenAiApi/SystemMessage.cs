using Clocktower.Game;
using System.ComponentModel;
using System.Text;

namespace Clocktower.OpenAiApi
{
    internal static class SystemMessage
    {
        public static string GetSystemMessage(string playerName, IReadOnlyCollection<string> playersNames, IReadOnlyCollection<Character> script)
        {
            return $"We are going to play a game of 'Blood on the Clocktower'. This is a social deduction game set in the town of Ravenswood Bluff where the players are divided into good Townsfolk and Outsiders and evil Minions and Demons. " +
            "The evil players know who is on which side and they win when there are only two players left alive, one of which is their Demon. The good players don't know who is on which side and must use their abilities and social skills " +
            "to determine who the evil players are. The good players win if the Demon dies, usually by executing them. The game is split into two phases: a day phase and a night phase.\r\n" +
            "During the day, you talk. You will each have a secret identity, a unique character from the script. Generally, the good players share whatever they know and attempt to find out who is who. Most good players will be telling the truth, " +
            "but some have an incentive to lie. If you are evil, you should definitely be lying (except in private conversations with your fellow evil players)! It is best to pick a good character to pretend to be, spreading as much false " +
            "information as possible. At the end of each day there will be a chance to nominate players for execution. Only one player may be executed each day, and the good team usually wants to execute a player each day since executing the " +
            "Demon is how they win. Each player only gets one nomination each day, and the same player can't be nominated more than once in a day. However each day, you may vote for as few or as many players as you wish, and whoever has the " +
            "most votes is executed. This player needs a vote tally of at least 50% of the living players or no execution occurs. On a tie, neither player is executed.\r\n" +
            "During the night, some players will get a chance to secretly use their ability or gain some type of information. This includes the Demon who can kill one player every night after the first. Most of you will die. " +
            "This is a good thing! In Ravenswood Bluff, death is not the end. Some players may even want to die, as they gain information when they do. If you are dead, you still participate in the game, you may still talk, and you still win or lose " +
            "when your team wins or loses.If you die, you are still a major part of the game. You still talk, and you still close your eyes during the night time. Most importantly, you still win or lose with your team. " +
            "In fact, the game is usually decided by the votes and opinions of the dead players. When you die, you lose your character ability, you may no longer nominate, and you have only one vote for the rest of the game, so use it wisely.\r\n" +
            "There is a lot of information in this game. However, some of it might be wrong. If you are drunk or poisoned, you have no ability, but I will pretend that you do. I am allowed to lie to you — any information that you get may be incorrect, " +
            "but you will not know if you are drunk or poisoned. For example, if you are the Drunk, you will draw a different character token out of the bag and only I know that you are actually the Drunk. Or, if the Poisoner poisons you, you may " +
            "still wake at night to use your ability, but it won’t work.\r\n" +
            "This game will use a script called 'A Simple Matter' which includes the following characters:\r\n" +
            ScriptToText(script) +
            SetupToText(playersNames.Count) +
            PlayersToText(playersNames) +
            $"You are {playerName}, a player in the game.\r\n";
        }

        private static string ScriptToText(IReadOnlyCollection<Character> script)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Townsfolk (good):");
            foreach (var townsfolk in script.Where(character => character.CharacterType() == CharacterType.Townsfolk))
            {
                sb.AppendLine(CharacterToText(townsfolk));
            }

            sb.AppendLine("Outsider (good):");
            foreach (var outsider in script.Where(character => character.CharacterType() == CharacterType.Outsider))
            {
                sb.AppendLine(CharacterToText(outsider));
            }

            sb.AppendLine("Minion (evil):");
            foreach (var minion in script.Where(character => character.CharacterType() == CharacterType.Minion))
            {
                sb.AppendLine(CharacterToText(minion));
            }

            sb.AppendLine("Demon (evil):");
            foreach (var demon in script.Where(character => character.CharacterType() == CharacterType.Demon))
            {
                sb.AppendLine(CharacterToText(demon));
            }


            return sb.ToString();
        }

        private static string CharacterToText(Character character)
        {
            return $"- {TextUtilities.CharacterToText(character)}: " + character switch
            {
                Character.Steward => "You start knowing 1 good player.",
                Character.Investigator => "You start knowing that 1 of 2 players is a particular Minion.",
                Character.Librarian => "You start knowing that 1 of 2 players is a particular Outsider. (Or that zero are in play)",
                Character.Shugenja => "You start knowing if your closest evil player is clockwise or anti-clockwise. If equidistant, this info is arbitrary.",
                Character.Empath => "Each night, you learn how many of your 2 alive neighbors are evil.",
                Character.Fortune_Teller => "Each night, choose 2 players: you learn if either is a Demon. There is 1 good player that registers falsely to you.",
                Character.Undertaker => "Each night (except the first night), you learn a character that died by execution today.",
                Character.Monk => "Each night (except the first night), choose a player (not yourself): they are safe from the Demon tonight.",
                Character.Fisherman => "Once per game, during the day, visit the Storyteller for some advice to help your team win.",
                Character.Slayer => "Once per game, during the day, publicly choose a player: if they are the Demon, they die.",
                Character.Philosopher => "Once per game, at night, choose a good character: gain that ability. If this character is in play, they are drunk.",
                Character.Soldier => "You are safe from the Demon.",
                Character.Ravenkeeper => "If you die at night, you are woken to choose a player: you learn their character.",

                Character.Tinker => "You might die at any time.",
                Character.Sweetheart => "When you die, 1 player is drunk from now on.",
                Character.Recluse => "You might register as evil and as a Minion or Demon, even if dead.",
                Character.Drunk => "You do now know you are the Drunk. You think you are a Townsfolk, but your ability malfunctions.",

                Character.Godfather => "You start knowing which Outsiders are in-play. If 1 died today, choose a player tonight: they die. [-1 or +1 Outsider]",
                Character.Poisoner => "Each night, choose a player: their ability malfunctions tonight and tomorrow day.",
                Character.Assassin => "Once per game, at night (except the first night), choose a player: they die, even if for some reason they could not.",
                Character.Scarlet_Woman => "If there are 5 or more players alive and the Demon dies, you become the Demon.",

                Character.Imp => "Each night (except the first night), choose a player: they die. If you choose yourself, you die and a Minion becomes the Imp.",

                _ => throw new InvalidEnumArgumentException(nameof(character))
            };
        }

        private static string SetupToText(int playerCount)
        {
            return $"In this game there are {playerCount} players. That means there will be {TownsfolkCount(playerCount)} Townsfolk, {OutsiderCount(playerCount)} Outsiders, {MinionCount(playerCount)} Minions and 1 Demon (unless modified by a Godfather).\r\n";
        }

        private static int TownsfolkCount(int playerCount)
        {
            return playerCount - (OutsiderCount(playerCount) + MinionCount(playerCount) + 1);
        }

        private static int OutsiderCount(int playerCount)
        {
            return (playerCount + 2) % 3;
        }

        private static int MinionCount(int playerCount)
        {
            return (playerCount - 4) / 3;
        }

        private static string PlayersToText(IReadOnlyCollection<string> playersNames)
        {
            var sb = new StringBuilder();

            sb.Append("In this game are the following players, going clockwise around town: ");
            foreach (var name in playersNames.SkipLast(1))
            {
                sb.Append(name);
                sb.Append(", ");
            }
            sb.Append(playersNames.Last());
            sb.AppendLine(". ");

            return sb.ToString();
        }
    }
}
