using Newtonsoft.Json;

namespace AgileEnginePhotos.Models
{
    public class PicturesMetadataModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("cropped_picture")]
        public string CroppedPicture { get; set; }
    }
}
