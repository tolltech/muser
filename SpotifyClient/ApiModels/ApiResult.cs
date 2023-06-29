using Newtonsoft.Json;

namespace Tolltech.SpotifyClient.ApiModels
{
    public class ApiResult<T>
    {
        [JsonProperty("result")]
        public T Resilt { get; set; }

        [JsonProperty("error")]
        public ApiError Error { get; set; }
    }
}
