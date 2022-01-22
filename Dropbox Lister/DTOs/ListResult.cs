using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dropbox_Lister.DTOs
{
    public class ListResult
    {
        [JsonProperty("entries")]
        public List<Content> Entries { get; set; }

        public string Cursor { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }
}
