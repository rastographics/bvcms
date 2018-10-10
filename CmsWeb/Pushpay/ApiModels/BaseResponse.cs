using System.Collections.Generic;
using Newtonsoft.Json;

namespace CmsWeb.Pushpay.ApiModels
{
    /// <summary>
    ///     Base class for our response methods, containing common properties
    /// </summary>
    public abstract class BaseResponse
    {
        [JsonProperty("_links")]
        public Dictionary<string, Link> Links { get; set; }
    }
}
