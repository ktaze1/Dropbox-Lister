using Newtonsoft.Json;
using System;

namespace Dropbox_Lister.DTOs.Exceptions
{
    public class GenericError : Exception
    {
        [JsonProperty("error_summary")]
        public string ErrorSummary { get; set; }

    }
}
