using Clocktower.Game;

namespace Clocktower.Agent.Notifier
{
    /// <summary>
    /// Responsible for sending notifications (in markup form) to a platform for display (for humans) or processing (for AI).
    /// The following markup can be explicitly used, though not all markup will necessarily be supported by all platforms.
    /// **text** for bold
    /// >>> text for quoted sections
    /// [color:green]text[/color] for colour
    /// </summary>
    internal interface IMarkupNotifier
    {
        /// <summary>
        /// Start a session on which notifications can be sent. Will be called exactly once before any other methods in the class.
        /// </summary>
        /// <param name="playerName">Name with which to identify the session.</param>
        Task Start(string name, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script);

        /// <summary>
        /// Sends a notification in markup form.
        /// </summary>
        /// <param name="markupText">A notification in markup form.</param>
        Task Notify(string markupText);

        /// <summary>
        /// Sends a notification in markup form with an accompanying image. Not all platforms will display the image.
        /// </summary>
        /// <param name="markupText">A notification in markup form.</param>
        /// <param name="imageFileName">The name of the image file to send.</param>
        Task NotifyWithImage(string markupText, string imageFileName);
    }
}
