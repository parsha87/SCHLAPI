namespace Scheduling.ViewModels
{
    public class VrtsettingViewModel
    {
        public int Id { get; set; }
        public int? NodeId { get; set; }
        public int? UpId { get; set; }
        public int? ProductType { get; set; }
        public int? ValveNo { get; set; }
        public int? ValveType { get; set; }
        public int? MasterNodeId { get; set; }
        public int? MasterValveNo { get; set; }
        public int? BlockNo { get; set; }
        public int? FertGrpNo { get; set; }
        public int? FilterGrpNo { get; set; }
        public int? LinkedSensor1NodeId { get; set; }
        public int? LinkedSensor1SensorNo { get; set; }
        public int? LinkedSensor2NodeId { get; set; }
        public int? LinkedSensor2SensorNo { get; set; }
        public int? ValveGrpNo1 { get; set; }
        public int? ValveGrpNo2 { get; set; }
        public int? HeadPumpGrNo { get; set; }
        public int? GwSrn { get; set; }
        public string TagNameValve { get; set; }
        public string TagNameMasterValve { get; set; }
        public string TagNameBlock { get; set; }
        public string TagNameFertGroup { get; set; }
        public string TagNameFilterGroup { get; set; }
        public string TagNameValveGroup { get; set; }
        public string TagNamePumpGroup { get; set; }
        public string TagNameMasterNode { get; set; }
        public int ValveStatus { get; set; }
        public int ValveReason { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
