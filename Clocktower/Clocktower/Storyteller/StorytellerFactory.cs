using Clocktower.Agent.Notifier;

namespace Clocktower.Storyteller
{
    internal static class StorytellerFactory
    {
        public static IStoryteller CreateLocalHumanStoryteller(Random random, bool autoAct = false)
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
    }
}
