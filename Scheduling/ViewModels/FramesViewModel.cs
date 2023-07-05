using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class FramesViewModel
    {

    }
    public class BODYBlank
    {
    }
    public class JoiningFrameViewModel
    {
        public virtual List<dynamic> HEAD { get; set; } = new List<dynamic>();
    }

    public class BODY
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> CS { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> VRT { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> SS { get; set; } = new List<dynamic>();
    }

    public class HeartBeatFrame
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<dynamic> HEAD { get; set; } = new List<dynamic>();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public BODYBlank BODY { get; set; }
    }


    public class PostEvents
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int MaxPageSize { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
    }

    public class PostEventsDataLlogger
    {
        public int GwidNo { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int MaxPageSize { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
    }


}
