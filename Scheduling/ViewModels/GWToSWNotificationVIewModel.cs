using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class APPUPID
    {
        public List<object> FILTER { get; set; }
    }

    public class NotificationGWTOSWBODY
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<APPUPID> APP_UPID { get; set; } = new List<APPUPID>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> NODE_UPID { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> SQ_UPID { get; set; } = new List<int>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MAIN_SCH_UPID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> DATA { get; set; } = new List<dynamic>();
    }

    public class GWToSWNotificationVIewModel
    {
        public List<dynamic> HEAD { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NotificationGWTOSWBODY BODY { get; set; } = new NotificationGWTOSWBODY();
    }


    public class NotificationResponseBODY
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> CS { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> VRT { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> SS { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> SQ_SET { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> SCH_UPID { get; set; } = new List<dynamic>();

    }

    public class SCHUPID
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int MAIN_SCH_UPID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NWTNO { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> UPID { get; set; }
    }

    class NWRTU
    {
        public int NWTNO { get; set; }
        public List<int> UPID { get; set; } = new List<int>();
    }

    class MschID
    {
        public int MAIN_SCH_UPID { get; set; }

    }
    public class BODYBlanks
    {
    }
    public class NotificationResponse
    {
        public List<object> HEAD { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public NotificationResponseBODY BODY { get; set; } = new NotificationResponseBODY();
        
    }

    public class NotificationResponseBlankBody
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<object> HEAD { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BODYBlanks BODY { get; set; } = new BODYBlanks();
    }
}
