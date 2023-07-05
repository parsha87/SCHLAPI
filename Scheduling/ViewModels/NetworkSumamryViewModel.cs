using Scheduling.Data.Entities;
using System.Collections.Generic;

namespace Scheduling.ViewModels
{
    public class NetworkSumamryViewModel
    {
       public List<MultiNodeNwDataFrameViewModel> NodeNwDataFrameList { get; set; } = new List<MultiNodeNwDataFrameViewModel>();
        
        public List<MultiHandShakeNonReach> MultiHandShakeNonReachList { get; set; } = new List<MultiHandShakeNonReach>();
        public List<MultiHandShakeReach> MultiHandShakeReachList { get; set; } = new List<MultiHandShakeReach>();
        public List<GwstatusData> GwstatusDataList { get; set; } = new List<GwstatusData>();

    }
}
