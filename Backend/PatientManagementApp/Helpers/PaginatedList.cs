namespace PatientManagementApp.Helpers
{
	public class PaginatedList<T>
	{
		public IEnumerable<T> Items { get; set; }
		public int PageIndex { get; set; }
		public int TotalPages { get; set; }
		public int TotalCount { get; set; }

		public PaginatedList(IEnumerable<T> items, int count, int pageIndex, int pageSize)
		{
			Items = items;
			TotalCount = count;
			PageIndex = pageIndex;
			TotalPages = (int)Math.Ceiling(count / (double)pageSize);
		}

		public bool HasPreviousPage => PageIndex > 1;
		public bool HasNextPage => PageIndex < TotalPages;
	}

}
