using System;
using System.Collections.Generic;
using System.Linq;

namespace Scheduling.ViewModels.Lib
{
    public class PagedList<T> : List<T>
    {
        private int count;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public bool HasPreviousPage => this.PageNumber > 1;
        public bool HasNextPage => this.PageNumber < this.TotalPages;
        public int NextPageNumber => this.HasNextPage ? this.PageNumber + 1 : this.TotalPages;
        public int PreviousPageNumber => this.HasPreviousPage ? this.PageNumber - 1 : 1;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalItems = count;
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int pageNumber, int pageSize, string orderby, bool orderdir)
        {
            var count = source.Count();
            if (orderdir)
            {
                var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }
            else
            {
                var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<T>(items, count, pageNumber, pageSize);
            }

            // return new PagedList<T>(items, count, pageNumber, pageSize);
        }
        public PagingHeader GetHeader()
        {
            return new PagingHeader(this.TotalItems, this.PageNumber, this.PageSize, this.TotalPages);
        }

    }
}
