using Newtonsoft.Json;

namespace AgileEnginePhotos.Models
{
    public class PicturesFullDataModel : PicturesMetadataModel
    {
        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("camera")]
        public string Camera { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("full_picture")]
        public string FullPicture { get; set; }
    }
}
