using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasNext => (CurrentPage < TotalPages);
        public bool HasPrevious => (CurrentPage > 1);

        public PagedList(List<T> collection, int currentPage, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double) TotalCount / pageSize);
            AddRange(collection);
        }

        public static PagedList<T> CreatePagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            List<T> collection = source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            return new PagedList<T>(collection, pageNumber, pageSize, count);
        }
    }
}
