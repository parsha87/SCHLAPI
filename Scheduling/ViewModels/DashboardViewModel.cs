using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class DashboardViewModel
    {

    }

    public class DashboardZoneMatrixModel
    {
        public List<KeyValueBlockSensorViewModel> blocks { get; set; } = new List<KeyValueBlockSensorViewModel>();
        public List<dynamic> fertStatus { get; set; } = new List<dynamic>();
        public List<dynamic> filterStatus { get; set; } = new List<dynamic>();
        public List<dynamic> masterValveStatus { get; set; } = new List<dynamic>();
        public List<dynamic> masterPumpStatus { get; set; } = new List<dynamic>();
        public List<dynamic> ruleStaus { get; set; } = new List<dynamic>();
        public List<dynamic> moStatus { get; set; } = new List<dynamic>();
        public List<dynamic> sensorThresholdStatus { get; set; } = new List<dynamic>();
        public List<dynamic> sensorStatus { get; set; } = new List<dynamic>();
        public List<dynamic> blockLevelSensors { get; set; } = new List<dynamic>();

    }


    public class DashboardBlockModel
    {
        public int SubblockId { get; set; }
        public int? BlockId { get; set; }
        public int? ZoneId { get; set; }
        public string Name { get; set; }
        public int? ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string TagName { get; set; }
        public string StartEndTime { get; set; }

        public string TimeTextColor { get; set; }
        public string TimeText { get; set; }
        public bool isHavingStartTime { get; set; }
        public string Attribute { get; set; }
        public string BackgroundImage { get; set; }
        public string ImageForDropKanis { get; set; }
        public string AttributeIcon { get; set; }
        public bool faultyvalve { get; set; }
        
    }

    public class DashboardBlockModelList
    {
       public List<DashboardBlockModel> dashboardBlockModelList { get; set; } = new List<DashboardBlockModel>();


    }

    public class BlockMatrixSPData
    {
        public SubBlockViewModel subblocks { get; set; } = new SubBlockViewModel();
        public StartEndTIme startEndTime { get; set; } = new StartEndTIme();
        public List<BstEvents> progressbar { get; set; } = new List<BstEvents>();
        public List<DashboardIcons> nextSchedule { get; set; } = new List<DashboardIcons>();
        public List<BstEvents> dashboardIcons { get; set; } = new List<BstEvents>();
        public List<string> channelNames { get; set; } = new List<string>();

        //public string channelNames { get; set; } 
    }


    public class StartEndTIme
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

    public class BstEvents
    {
        public string networkname { get; set; }
        public int bstid { get; set; }
        public int bstbatvolt { get; set; }
        public DateTime actualeventdatetime { get; set; }
        public string eventdatetime { get; set; }
        public string elenumberandtype { get; set; }
        public string ElementType { get; set; }
        public int number { get; set; }
        public string elestatus { get; set; }
        public int reasonId { get; set; }
        public string reason { get; set; }
        public int errorcode { get; set; }
        public string Error { get; set; }
        public int rtuNo { get; set; }
        public string receiveddatetime { get; set; }

    }

    public class DashboardIcons
    {
        public int ValveTimespanId { get; set; }
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public int ChannelId { get; set; }
        public DateTime SeqDate { get; set; }
        public int StartTimeSpanId { get; set; }
        public int EndTimeSpanId { get; set; }
        public int LoopingSeqId { get; set; }
        public int IsCallAlert { get; set; }
        public int IsSchAlert { get; set; }
        public string startTime { get; set; }
        public string EndTime { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class FertStatus
    {
        public string groupname { get; set; }
        public DateTime eventdatetime { get; set; }
        public string ChannelName { get; set; }
        public string error { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public int blockid { get; set; }
        public int color { get; set; }

    }

    public class FilterStatus
    {
        public string groupname { get; set; }
        public DateTime eventdatetime { get; set; }
        public string ChannelName { get; set; }
        public string error { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public int blockid { get; set; }
        public int color { get; set; }

    }

    public class MastererValveStatus
    {
        public string groupname { get; set; }
        public DateTime eventdatetime { get; set; }
        public string ChannelName { get; set; }
        public string error { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public int blockid { get; set; }
        public int color { get; set; }

    }

    public class MasterPumpStatus
    {
        public string groupname { get; set; }
        public DateTime eventdatetime { get; set; }
        public string ChannelName { get; set; }
        public string error { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public int blockid { get; set; }
        public int color { get; set; }

    }
    public class RuleStatus
    {
        public string rulename { get; set; }
        public DateTime executionDate { get; set; }
        public int rulestatus { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public int blockid { get; set; }
        public int color { get; set; }

    }


}
