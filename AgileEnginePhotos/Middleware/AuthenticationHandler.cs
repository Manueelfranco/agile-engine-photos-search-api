using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API.Middleware
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        private ILogger<AuthenticationHandler> logger;

        public AuthenticationHandler(IConfiguration configuration, IMemoryCache memoryCache, ILogger<AuthenticationHandler> logger)
        {
            this.configuration = configuration;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage = null;

            if (memoryCache.TryGetValue("AuthToken", out string authToken))
            {
                logger.LogInformation("Auth token pulled from memory cache successfully");
                responseMessage = await SendRequestAsync(request, authToken, cancellationToken);
            }

            // Either if token is not present in the cache, or if it was not valid due to 401/403 responses (it may have expired), we need to request it again.
            if (responseMessage == null ||
                responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                responseMessage.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                authToken = await RequestAuthTokenAsync();
                responseMessage = await SendRequestAsync(request, authToken, cancellationToken);
            }

            return responseMessage;
        }

        private async Task<string> RequestAuthTokenAsync()
        {
            try
            {
                logger.LogInformation("Getting auth token from AgileEngine API");
                var authToken = string.Empty;

                using (HttpClient httpClient = new HttpClient())
                {
                    string content = JsonConvert.SerializeObject(new { apiKey = configuration["ApiKey"] });
                    var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("http://interview.agileengine.com/auth", httpContent);

                    response.EnsureSuccessStatusCode();
                    authToken = JsonConvert.DeserializeObject<AuthModel>(await response.Content.ReadAsStringAsync()).Token;
                }

                logger.LogInformation("Auth token retrieved successfully. Setting memory cache");
                memoryCache.Set("AuthToken", authToken); // To avoid doing this again in the future until the token expires
                return authToken;
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error while trying to get the auth token from the AgileEngine API. Details: {exceptionDetails}", ex.Message);
                throw;
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, string authToken, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
