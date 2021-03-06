using System.Net.Http;
using Newtonsoft.Json.Linq;
using ValidaSenha.Api.Utils;

namespace Newtonsoft.Json
{
    public static class HttpContentExtensions
    {
        public static JToken AsJToken(this HttpContent httpContent) =>
             JsonConvert.DeserializeObject<JToken>(
                 value: httpContent.ReadAsStringAsync().GetAwaiter().GetResult(),
                 settings: JsonUtils.GetJsonSerializerSettings()
                 );
        
        public static JToken AsJToken(this HttpRequestMessage httpRequestMessage) =>
             JToken.FromObject(httpRequestMessage, JsonUtils.GetJsonSerializer());

        public static JToken AsJToken(this HttpResponseMessage httpResponseMessage)
        {
            return new JObject(
                new JProperty("StatusCode", httpResponseMessage.StatusCode),
                new JProperty("ReasonPhrase", httpResponseMessage.ReasonPhrase),
                new JProperty("Headers", httpResponseMessage.Headers),
                new JProperty("Content", httpResponseMessage.Content.AsJToken())
            );
        }
             
    }
}