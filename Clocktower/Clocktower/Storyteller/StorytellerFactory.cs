using Clocktower.Agent;
using Clocktower.Agent.Notifier;
using Clocktower.Game;
using OpenAi;

namespace Clocktower.Storyteller
{
    internal static class StorytellerFactory
    {
        public static IStoryteller CreateStoryteller(IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script, Random random, string? aiModel)
        {
            if (aiModel == null)
            {   // Human Storyteller
                return CreateLocalHumanStoryteller(random);
            }
            else
            {   // AI Storyteller
                return CreateRobotStoryteller(aiModel, playerNames, scriptName, script);
            }
        }

        private static IStoryteller CreateLocalHumanStoryteller(Random random, bool autoAct = false)
        {
            var form = new StorytellerForm(random)
            {
                AutoAct = autoAct
            };
            var notifier = new RichTextBoxNotifier(form.Output);
            var storyteller = new TextStoryteller(notifier);
            storyteller.OnStartGame += () =>
            {
                form.Show();
                return Task.CompletedTask;
            };
            storyteller.SendMarkupText += notifier.AddToTextBox;
            storyteller.PromptMarkupText += form.Prompt;
            storyteller.PromptForTextResponse += form.PromptForText;

            return storyteller;
        }

        private static IStoryteller CreateRobotStoryteller(string model, IReadOnlyCollection<string> playerNames, string scriptName, IReadOnlyCollection<Character> script)
        {
            var chat = new OpenAiChat(model);
            var chatAiStoryteller = new ChatAiStoryteller(chat, playerNames, scriptName, script);
            var notifier = new RawOpenAiNotifier(chat);
            var storyteller = new TextStoryteller(notifier);
            var form = new AiStorytellerForm();

            chat.OnChatMessageAdded += (_, role, message) => form.OnChatMessage(role, message);
            chat.OnAssistantRequest += (_, _, _, _, promptTokens, completionTokens, totalTokens) => form.OnTokenCount(promptTokens, completionTokens, totalTokens);

            storyteller.OnStartGame += () =>
            {
                form.Show();
                return Task.CompletedTask;
            };
            storyteller.SendMarkupText += chat.AddUserMessage;
            storyteller.PromptMarkupText += chatAiStoryteller.Prompt;
            storyteller.PromptForTextResponse += chatAiStoryteller.PromptForText;

            return storyteller;
        }
    }
}
