using KuttSharp.Models;
using KuttSharp.Models.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KuttSharp
{
    /// <summary>
    /// Provides an interface for calling Kutt.it url shortening services (v2 API).
    /// </summary>
    public class KuttApiV2
    {
        private const string KUTT_DEFAULT_SERVER = "https://kutt.it";
        private const string API_KEY_HEADER = "x-api-key";

        private const string LINKS_URL = "/api/v2/links";

        private HttpClient Client;

        private readonly JsonSerializerSettings jsonSerializationSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly JsonSerializerSettings jsonDeserializationSettings = new JsonSerializerSettings()
        {
            Converters = { new IsoDateTimeConverter() }
        };

        /// <summary>
        /// Determines the server that the requests send to
        /// </summary>
        public Uri KuttServer { get; }

        /// <summary>
        /// ApiKey used to authorize requests to the KuttServer service
        /// </summary>
        public string ApiKey { get; }

        private Uri CreateLinkUri => new Uri($"{KuttServer.OriginalString}{LINKS_URL}");

        /// <summary>
        /// Creates an Api which communicates with the default Kutt server
        /// </summary>
        /// <param name="apiKey"></param>
        public KuttApiV2(string apiKey)
            : this(apiKey, KUTT_DEFAULT_SERVER, new HttpClient())
        { }

        /// <summary>
        /// Creates an Api which communicates with a self-hosted Kutt server
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="server">Determines the server that the requests send to</param>
        public KuttApiV2(string apiKey, Uri server)
            : this(apiKey, server, new HttpClient())
        { }

        /// <summary>
        /// Creates an Api which communicates with a self-hosted Kutt server
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="server">Determines the server that the requests send to</param>
        public KuttApiV2(string apiKey, string server)
            : this(apiKey, server, new HttpClient())
        { }

        /// <summary>
        /// Creates an Api which communicates with a self-hosted Kutt server
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="server">Determines the server that the requests send to</param>
        /// <param name="client">Use custom httpClient for request (useful for proxy or other modifications to the client handler)</param>
        public KuttApiV2(string apiKey, string server, HttpClient client)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            KuttServer = new Uri(server) ?? throw new ArgumentNullException(nameof(server));
            Client = client ?? throw new ArgumentNullException(nameof(client));

            if (!Client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                Client.DefaultRequestHeaders.Add(API_KEY_HEADER, apiKey);
        }

        /// <summary>
        /// Creates an Api which communicates with a self-hosted Kutt server
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="server">Determines the server that the requests send to</param>
        /// <param name="client">Use custom httpClient for request (useful for proxy or other modifications to the client handler)</param>
        public KuttApiV2(string apiKey, Uri server, HttpClient client)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            KuttServer = server ?? throw new ArgumentNullException(nameof(server));
            Client = client ?? throw new ArgumentNullException(nameof(client));

            if (!Client.DefaultRequestHeaders.Contains(API_KEY_HEADER))
                Client.DefaultRequestHeaders.Add(API_KEY_HEADER, apiKey);
        }

        /// <summary>
        /// Create a link
        /// </summary>
        /// <param name="target">Target URL to be shortened</param>
        /// <param name="password">Set a password</param>
        /// <param name="customUrl">Set a custom URL</param>
        /// <param name="reuse">If a URL with the specified target exists returns it, otherwise will send a new shortened URL. Default is false.</param>
        /// <param name="domain">Set a custom domain</param>
        /// <returns>Created link as a <see cref="KuttLink"/></returns>
        public async Task<KuttLink> CreateLinkAsync(string target,
            string password = null,
            string customUrl = null,
            bool reuse = false,
            string domain = null)
        {
            // Null and empty string values will error out
            var request = new
            {
                Target = target,
                Reuse = reuse,
                Password = password?.Length > 0 ? password : null,
                Customurl = customUrl?.Length > 0 ? customUrl : null,
                Domain = domain?.Length > 0 ? domain : null
            };

            var json = JsonConvert.SerializeObject(request, jsonSerializationSettings);
            var body = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync(CreateLinkUri, body).ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<KuttLink>(responseString, jsonDeserializationSettings);
            }
            else
            {
                try
                {
                    var errMessage = JsonConvert.DeserializeObject<KuttError>(responseString).Error;
                    throw new KuttException(errMessage);
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}