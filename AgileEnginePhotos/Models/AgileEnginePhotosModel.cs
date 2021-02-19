using Newtonsoft.Json;

namespace AgileEnginePhotos.Models
{
    public class AgileEnginePhotosModel
    {
        [JsonProperty("pictures")]
        public PicturesMetadataModel[] Pictures { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }
        
        [JsonProperty("pageCount")]
        public int PageCount { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }
    }
}
