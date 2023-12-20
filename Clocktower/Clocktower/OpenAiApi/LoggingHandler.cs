namespace Clocktower.OpenAiApi
{
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(TextWriter stream, HttpMessageHandler innerHandler)
        : base(innerHandler)
        {
            this.stream = stream;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            stream.WriteLine("Request:");
            stream.WriteLine(request.ToString());
            if (request.Content != null)
            {
                stream.WriteLine(await request.Content.ReadAsStringAsync(cancellationToken));
            }
            stream.WriteLine(string.Empty);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            stream.WriteLine("Response:");
            stream.WriteLine(response.ToString());
            if (response.Content != null)
            {
                stream.WriteLine(await response.Content.ReadAsStringAsync(cancellationToken));
            }
            stream.WriteLine(string.Empty);

            return response;
        }

        private readonly TextWriter stream;
    }
}
