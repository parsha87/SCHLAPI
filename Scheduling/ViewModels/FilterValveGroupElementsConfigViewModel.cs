using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class FilterValveGroupElementsConfigViewModel
    {
        public int FltgrEleConfigId { get; set; }
        public int MstfilterGroupId { get; set; }
        public int? ChannelId { get; set; }
        public int? OpSeqOfFlush { get; set; }
        public int? PressSustainingOpNo { get; set; }
        public string ChannelName { get; set; }
        public string PressSustainingOpChannelName { get; set; }

    }
}
