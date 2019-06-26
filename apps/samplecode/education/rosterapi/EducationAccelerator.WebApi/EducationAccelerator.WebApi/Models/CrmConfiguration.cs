using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EducationAccelerator.Models
{
    public class CrmConfiguration : BaseModel
    {
        public static readonly string EntityName = "msk12_configuration";
        public static readonly string EntitySetName = "msk12_configurations";

        public Guid msk12_configurationid { get; set; }
        public string msk12_apiurl { get; set; }
        public string msk12_consumerkey { get; set; }
        public string msk12_consumersecret { get; set; }
        public DateTime? msk12_lastsyncdate { get; set; }

        public static readonly Dictionary<string, string> Fields = new Dictionary<string, string>()
        {
            ["msk12_apiurl"] = "msk12_apiurl",
            ["msk12_configurationid"] = "msk12_configurationid",
            ["msk12_lastsyncdate"] = "msk12_lastsyncdate",
            ["msk12_name"] = "msk12_name",
            ["msk12_consumerkey"] = "msk12_consumerkey",
            ["msk12_consumersecret"] = "msk12_consumersecret"
        };

        internal override string ModelType()
        {
            throw new NotImplementedException();
        }

        internal override string UrlType()
        {
            throw new NotImplementedException();
        }

        public string ToJson()
        {
            var sb = new StringBuilder();
            using (var writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("msk12_configurationid");
                writer.WriteValue(msk12_configurationid);

                writer.WritePropertyName("msk12_lastsyncdate");
                writer.WriteValue(msk12_lastsyncdate?.ToString("o"));

                writer.WriteEndObject();
                writer.Flush();
            }
            return sb.ToString();
        }
    }
}
