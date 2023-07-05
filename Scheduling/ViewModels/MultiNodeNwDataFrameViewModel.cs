using System;

namespace Scheduling.ViewModels
{
    public class MultiNodeNwDataFrameViewModel
    {
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        public string LastCommTime { get; set; }
        public int? RxFrametype { get; set; }
      
        public int? NgcurrentRssi { get; set; }
      
        public int? NgcurrentSnr { get; set; }
     
        public int? NgcurrentSf { get; set; }
    
        public int? Ngfreq { get; set; }
      
        public int? NgcurrentCr { get; set; }
        
        public int? Gwid6b { get; set; }
      
        public int? Ngattempt3b { get; set; }
        public int? Power2b { get; set; }
     
        public int? Sfgw2b { get; set; }
       
        public int? GnsnrPrevious3b { get; set; }
      
        public int? GnrssiPrevious3b { get; set; }
        public int? ProductType4b { get; set; }       
        public DateTime AddedDatetime { get; set; }
        public int? GwLastCommTime { get; set; }
        public int Arrow { get; set; }
    }
}
