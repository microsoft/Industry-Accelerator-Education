using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EducationAccelerator.WebApi.Serializers
{
    public class OneRosterDeserializer
    {
        public static List<T> Deserialize<T>(string json)
        {
            var result = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(json, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });

            return result.Values.First();
        }

        private static void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
