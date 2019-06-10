using System.Collections.Generic;
using Newtonsoft.Json;
using TransactionGateway.ApiModels;

namespace TransactionGateway.ApiModels
{
    /// <summary>
    ///     Base class for our response methods, containing common properties
    /// </summary>
    public abstract class BaseResponse
    {
        [JsonProperty("_links")]
        public Dictionary<string, Link> Links { get; set; }
        public int? Page { get; set; }
        public int? TotalPages { get; set; }
        public int? PageSize { get; set; }
    }
}
