using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KuttSharp.Models.V2
{
    /// <summary>
    /// Represents a Kutt Link object.
    /// </summary>
    public class KuttLink
    {
        /// <summary>
        /// Unique ID of the link
        /// </summary>
        [JsonProperty("id")]
        [JsonRequired]
        public string Id { get; set; }

        /// <summary>
        /// Where the link will redirect to
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// Whether or not a password is required
        /// </summary>
        [JsonProperty("password")]
        public bool IsPasswordRequired { get; set; }

        /// <summary>
        /// Whether or not this link is banned
        /// </summary>
        [JsonProperty("banned")]
        public bool IsBanned { get; set; }

        /// <summary>
        /// The link address (ID portion of the link)
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The shortened link
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; }

        /// <summary>
        /// The custom domain used for this link, if set
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// The amount of visits to this link
        /// </summary>
        [JsonProperty("visit_count")]
        public long Visits { get; set; }

        /// <summary>
        /// When the link was created
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// When the link was updated
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
