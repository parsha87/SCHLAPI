using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class GroupDetailsViewModel
    {
        public MultiGroupMasterViewModel MultiGroupMaster { get; set; } = new MultiGroupMasterViewModel();
        public List<MultiGroupDataViewModel> MultiGroupData { get; set; } = new List<MultiGroupDataViewModel>();
    }

    public partial class MultiGroupMasterViewModel
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Tagname { get; set; }
        public bool? IsActive { get; set; }
    }
    public partial class MultiGroupDataViewModel
    {
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string SensorName { get; set; }
        public string SensorTagName { get; set; }
        public int? ProductType { get; set; }
        public int? Gwsrn { get; set; }
        public int? NodeProductId { get; set; }
        public int? NodeId { get; set; }
        public int? Ssno { get; set; }
        public int? ConfigurationId { get; set; }
        public int? Priority { get; set; }
    }
}
