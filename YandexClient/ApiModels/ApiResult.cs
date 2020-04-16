using Newtonsoft.Json;

namespace Tolltech.YandexClient.ApiModels
{
    public class ApiResult<T>
    {
        [JsonProperty("result")]
        public T Resilt { get; set; }

        [JsonProperty("error")]
        public ApiError Error { get; set; }
    }
}
