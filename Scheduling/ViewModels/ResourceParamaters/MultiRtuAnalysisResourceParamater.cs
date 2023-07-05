using System;

namespace Scheduling.ViewModels.ResourceParamaters
{
    public class MultiRtuAnalysisResourceParamater : QueryStringParameters
    {
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
    }

    public class ResourceParameter : QueryStringParameters
    {

    }
}
