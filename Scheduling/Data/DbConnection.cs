using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Scheduling.Data
{
    public class DbConnection
    {
        [JsonProperty("siteName")]
        public string SiteName { get; set; }

        [JsonProperty("dbconnection")]
        public string DefaultConnection { get; set; }

        [JsonProperty("dbconnectionevents")]
        public string DefaultConnectionEvents { get; set; }

        [JsonProperty("dbconnectiontimestamp")]
        public string DefaultConnectionTimeStamp { get; set; }

        public static List<DbConnection> FromJson(string json) => JsonConvert.DeserializeObject<List<DbConnection>>(json, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}

