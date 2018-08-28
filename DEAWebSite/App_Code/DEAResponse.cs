using System;
using DEALib;
using Newtonsoft.Json;

namespace DEAService.Responses
{
    public class DEAResponse
    {
        [JsonProperty (PropertyName = "OK", Order = 10)]
        public bool OK { get; set; }

        [JsonProperty(PropertyName = "ErrorMessage", Order = 20, NullValueHandling = NullValueHandling.Ignore)]
        public string errorMessage { get; set; }

        [JsonProperty(PropertyName = "Context", Order = 30, NullValueHandling = NullValueHandling.Ignore)]
        public DEAContext context { get; set; }
    }
}