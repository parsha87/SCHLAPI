namespace Scheduling.ViewModels
{
    public abstract class QueryStringParameters
    {
		public int MaxPageSize { get; set; }
		public string SearchQuery { get; set; }
		public int PageNumber { get; set; }
		public string OrderBy { get; set; }
		public string OrderDir { get; set; }

		private int _pageSize;
		public int PageSize
		{
			get => _pageSize;
			set =>_pageSize = (value > MaxPageSize) ? MaxPageSize : value;			
		}
	}
}
