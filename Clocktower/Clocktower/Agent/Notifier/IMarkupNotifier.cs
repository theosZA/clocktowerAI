using Clocktower.Game;

namespace Clocktower.Agent.Notifier
{
    /// <summary>
    /// Responsible for sending notifications (in markup form) to a platform for display (for humans) or processing (for AI).
    /// </summary>
    internal interface IMarkupNotifier
    {
        /// <summary>
        /// Start a session on which notifications can be sent. Will be called exactly once before any other methods in the class.
        /// </summary>
        /// <param name="playerName">Name with which to identify the session.</param>
        Task Start(string name, IReadOnlyCollection<string> players, string scriptName, IReadOnlyCollection<Character> script);

        /// <summary>
        /// Sends a notification in markup form. Not all markup will necessarily be supported by the platform receiving the notification.
        /// </summary>
        /// <param name="markupText">A notification in markup form.</param>
        Task Notify(string markupText);

        /// <summary>
        /// Queues a notification in markup form. The notification need not be dispatched immediately to the platform, and so is useful
        /// for smaller notifications that can be batched together and all sent at once on the next non-queued notify or other action on
        /// the platform.
        /// </summary>
        /// <param name="markupText">A notification in markup form.</param>
        void QueueNotify(string markupText);

        /// <summary>
        /// Sends a notification in markup form with an accompanying image. Not all platforms will display the image.
        /// </summary>
        /// <param name="markupText">A notification in markup form.</param>
        /// <param name="imageFileName">The name of the image file to send.</param>
        Task NotifyWithImage(string markupText, string imageFileName);
    }
}
