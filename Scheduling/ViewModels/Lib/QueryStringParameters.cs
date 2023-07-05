namespace Scheduling.ViewModels.Lib
{
	public abstract class QueryStringParameters
	{
		const int maxPageSize = 10;
		public string SearchQuery { get; set; }
		public int PageNumber { get; set; }
		public string OrderBy { get; set; }
		public string OrderDir { get; set; }

		private int _pageSize;
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
		}
	}
}
