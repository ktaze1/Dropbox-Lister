using Newtonsoft.Json;

namespace Dropbox_Lister.DTOs
{
    public class Content
    {
        [JsonProperty(".tag")]
        public string Tag { get; set; }

        public string Name { get; set; }

        public long Size { get; set; }

        [JsonProperty("path_lower")]
        public string Path { get; set; }
    }
}
