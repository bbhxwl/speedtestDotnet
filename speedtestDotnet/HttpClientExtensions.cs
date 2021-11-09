using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        private const int BufferSize = 8192;

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient client, Uri requestUri, IProgress<HttpDownloadProgress> progress, CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            using (var responseMessage = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                responseMessage.EnsureSuccessStatusCode();

                var content = responseMessage.Content;
                if (content == null)
                {
                    return Array.Empty<byte>();
                }

                var headers = content.Headers;
                var contentLength = headers.ContentLength;
                using (var responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var buffer = new byte[BufferSize];
                    int bytesRead;
                    var bytes = new List<byte>();

                    var downloadProgress = new HttpDownloadProgress();
                    if (contentLength.HasValue)
                    {
                        downloadProgress.TotalBytesToReceive = (ulong)contentLength.Value;
                    }
                    progress?.Report(downloadProgress);

                    while ((bytesRead = await responseStream.ReadAsync(buffer, 0, BufferSize, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        bytes.AddRange(buffer.Take(bytesRead));

                        downloadProgress.BytesReceived += (ulong)bytesRead;
                        progress?.Report(downloadProgress);
                    }

                    return bytes.ToArray();
                }
            }
        }
    }

    public struct HttpDownloadProgress
    {
        public ulong BytesReceived { get; set; }

        public ulong? TotalBytesToReceive { get; set; }
    }
}
