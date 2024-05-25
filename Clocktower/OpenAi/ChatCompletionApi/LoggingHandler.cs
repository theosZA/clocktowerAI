namespace OpenAi.ChatCompletionApi
{
    internal class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(TextWriter stream, HttpMessageHandler innerHandler)
        : base(innerHandler)
        {
            this.stream = TextWriter.Synchronized(stream);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await stream.WriteLineAsync("Request:".AsMemory(), cancellationToken);
            await stream.WriteLineAsync(request.ToString().AsMemory(), cancellationToken);
            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
                await stream.WriteLineAsync(requestContent.AsMemory(), cancellationToken);
            }
            await stream.WriteLineAsync(string.Empty.AsMemory(), cancellationToken);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            await stream.WriteLineAsync("Response:".AsMemory(), cancellationToken);
            await stream.WriteLineAsync(response.ToString().AsMemory(), cancellationToken);
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                await stream.WriteLineAsync(responseContent.AsMemory(), cancellationToken);
            }
            await stream.WriteLineAsync(string.Empty.AsMemory(), cancellationToken);

            return response;
        }

        private readonly TextWriter stream;
    }
}
