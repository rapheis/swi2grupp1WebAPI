using Newtonsoft.Json;

namespace swi2grupp1WebAPI.Data
{
    public class FilmImage
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }

        // Images ist in Base64String
        public string? Image { get; set; }
        public string? Preview { get; set; }
    }
}
