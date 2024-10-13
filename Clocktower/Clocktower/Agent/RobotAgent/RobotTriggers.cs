using Clocktower.Game;

namespace Clocktower.Agent.RobotAgent
{
    /// <summary>
    /// Methods that should be called at certain points in the game, that will trigger additional calls to the Robot.
    /// These are a collection of administrative triggers (e.g. tracking character, splitting AI-chat into day and
    /// night segments) and AI prompting triggers which allows us to get the AI's reasoning on the current game state.
    /// </summary>
    public class RobotTriggers
    {
        public Action? OnStatusChange { get; set; }

        public string PlayerName { get; private set; }

        public Character? Character { get; private set; }
        public Character? OriginalCharacter { get; private set; }

        public Alignment Alignment { get; private set; } 

        public bool Alive { get; private set; } = true;

        public RobotTriggers(string playerName, ClocktowerChatAi clocktowerChat)
        {
            PlayerName = playerName;
            this.clocktowerChat = clocktowerChat;
        }

        public async Task OnDay(int dayNumber)
        {
            if (dayNumber == 1)
            {
                await PromptForBluff();
            }
            await clocktowerChat.Day(dayNumber);
        }

        public async Task OnNight(int nightNumber)
        {
            await clocktowerChat.Night(nightNumber);
        }

        public async Task OnNominationStart(int numberOfLivingPlayers)
        {
            await PromptForOverview();
            if (numberOfLivingPlayers <= 4)
            {   // Down to what may well be the final round of nominations, we want our AI to express who it thinks is the demon, and so encourage it to try sway the rest of town and hopefully vote that way if they still have a vote left.
                await PromptForDemonGuess();
            }
        }

        public Task OnAssignCharacter(Character character, Alignment alignment)
        {
            Character = character;
            Alignment = alignment;
            OnStatusChange?.Invoke();

            return Task.CompletedTask;
        }

        public Task OnChangeAlignment(Alignment alignment)
        {
            Alignment = alignment;
            OnStatusChange?.Invoke();

            return Task.CompletedTask;
        }

        public Task OnGainingCharacterAbility(Character character)
        {
            OriginalCharacter = Character;
            Character = character;
            OnStatusChange?.Invoke();

            return Task.CompletedTask;
        }

        public Task OnDead()
        {
            Alive = false;
            clocktowerChat.AddFormattedMessage("*A reminder that while dead, you still participate in the game, you may still talk, and you still win or lose with your team. In fact, the game is usually decided by the votes " +
                                               "and opinions of the dead players. You no longer have your character ability, you may no longer nominate, and you have only one vote for the rest of the game, so use it wisely.*");
            OnStatusChange?.Invoke();

            return Task.CompletedTask;
        }

        public Task YourDemonIs(Player demon)
        {
            this.demon = demon;

            return Task.CompletedTask;
        }

        public Task YourMinionsAre(IReadOnlyCollection<Player> minions)
        {
            this.minions = minions;

            return Task.CompletedTask;
        }

        public async Task EndGame()
        {
            await clocktowerChat.Request("With the game concluded, do you have any final thoughts to share? Any surprises in the reveal of which player was which character? And do you think you could have improved any aspects of your own gameplay?");
        }

        private async Task PromptForBluff()
        {
            if (Character == null)
            {
                return;
            }
            else if (Character.Value.CharacterType() == CharacterType.Demon)
            {
                await clocktowerChat.Request("Which good character will you bluff as to the good players? You have been provided a selection of characters that are not in play that you can safely use for bluffs, or you could bluff as another " +
                                             "character on the script at the risk of double-claiming that character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to the good players.");
                if (minions != null)
                {
                    if (minions.Count > 1)
                    {
                        clocktowerChat.AddFormattedMessage("Remember that your minions (%P) may also need safe characters to bluff as. Consider having private conversations with them during the day.", minions);
                    }
                    else
                    {
                        clocktowerChat.AddFormattedMessage("Remember that your minion (%P) may also need a safe character to bluff as. Consider having a private conversation with them during the day.", minions);
                    }
                }
            }
            else if (Character.Value.CharacterType() == CharacterType.Minion)
            {
                if (Character == Game.Character.Godfather)
                {
                    await clocktowerChat.Request("Which good character will you bluff as to the good players? As the %c you know which Outsiders are in play and therefore which ones are safe to use as a bluff. Or you could bluff as a Townsfolk " +
                                                 "character on the script at the risk of double-claiming that character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to the good players.", Game.Character.Godfather);
                }
                else
                {
                    await clocktowerChat.Request("Which good character will you bluff as to the good players? You may also hedge your bets by claiming 2 or 3 different characters to the good players.");
                }
                if (demon != null)
                {
                    clocktowerChat.AddFormattedMessage("Remember thar your demon (%p) has received a selection of safe characters to bluff. Consider having a private conversation with them during the day and see if you want to revise your bluff.", demon);
                }
            }
            else  // good player
            {
                await clocktowerChat.Request("As a good player you may choose to be open about your character. However there are reasons that you may want to bluff about which character you are (a character that benefits from being targeted by " +
                                             "the demon at night may want to bluff as a strong recurring information-gaining character to bait an attack by the demon, or vice-versa), or only inform a limited selection of players " +
                                             "about your true character. Alternatively you could hedge your bets by claiming 2 or 3 different characters to each player you talk to. What will your approach be?");
                clocktowerChat.AddMessage("Regardless of whether you bluff or not, remember that as a good player, you will want to be honest about your character and your information towards the end of the game (when there are only " +
                                          "3 or 4 players left alive).");
            }
        }

        private async Task PromptForOverview()
        {
            await clocktowerChat.RequestReasoning(Prompts.GetReasoningPrompt());
        }

        private async Task PromptForDemonGuess()
        {
            if (Character == null)
            {
                return;
            }
            if (Character.Value.Alignment() == Alignment.Evil)
            {
                await clocktowerChat.RequestReasoning("This may be the last day for nominations. Which of the living players, other than the demon, do you think you can convince the good players is the actual demon?");
            }
            else // Alignment is Good
            {
                await clocktowerChat.RequestReasoning("This may be the last day for nominations. Which of the living player do you think is most likely to be the demon, and why? " +
                                                      "How do you think you can convince the rest of your fellow good players to vote to execute them?");
            }
        }

        private readonly ClocktowerChatAi clocktowerChat;

        private IReadOnlyCollection<Player>? minions;
        private Player? demon;
    }
}
