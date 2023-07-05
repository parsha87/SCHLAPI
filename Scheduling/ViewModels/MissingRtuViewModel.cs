using System;

namespace Scheduling.ViewModels
{
    public class MissingRtuViewModel
    {
        public int Id { get; set; }
        public int? NetworkNo { get; set; }
        public string FrameName { get; set; }
        public int? NodeId { get; set; }
        public DateTime? AddedDateTime { get; set; }
    }
}
