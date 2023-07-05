using Scheduling.Data.Entities;
using System.Collections.Generic;

namespace Scheduling.ViewModels
{
    public class MultiNodeDashbordData
    {
        public int? NetworkNo { get; set; }
        public int? NodeId { get; set; }
        public int? NodeNo { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public List<VrtsettingViewModel> VrtList { get; set; } = new List<VrtsettingViewModel>();
        public List<SensorListModel> SensorList { get; set; } = new List<SensorListModel>();
    }


    public class SensorListModel
    {
        public int? Id { get; set; }
        public int? ProductType { get; set; }
        public int? GwSrn { get; set; }
        public int? NodePorductId { get; set; }
        public string TagName { get; set; }
        public int? NodeId { get; set; }
        public int? SsNo { get; set; }
        public decimal? SensorValue { get; set; }

        public int? SensorType { get; set; }
        public decimal? ScaleMin { get; set; }

        public decimal? ScaleMax { get; set; }

        public decimal? PulseValue { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }


}
