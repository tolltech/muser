using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class ApiError
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}