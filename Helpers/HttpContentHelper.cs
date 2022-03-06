using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace MyPharmacyIntegrationTests.Helpers
{
    public static class HttpContentHelper
    {
        public static HttpContent SerializeToJson(object toJsonObject)
        {
            var newSerializedObject = JsonConvert.SerializeObject(toJsonObject);
            var newHttpContent = new StringContent(newSerializedObject, UnicodeEncoding.UTF8, "application/json");
            return newHttpContent;
        }
    }
}
