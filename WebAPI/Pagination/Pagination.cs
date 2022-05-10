using System;
using System.Linq;
using System.Linq.Expressions;
using AsKMe.Pagination;
namespace Askme.Pagination
{
    public static class Pagination
    {
        /// <summary>
        /// Create Pagination Result from IQueryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The sequence where the elements are from</param>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Number of items to take per page</param>
        /// <returns>An IPaginationResult than contains a selected number of elements from the source and the total number of pages.</returns>
        public static PaginationResult<T> ToPagination<T>(this IQueryable<T> query, int page, int pageSize)
        {
            PaginationResult<T> paginationResult = new PaginationResult<T>()
            {
                TotalPages = query.GetTotalPages(pageSize),
                TotalElements = query.Count(),
                Content = query.SetPageData(page, pageSize).ToList()
            };

            return paginationResult;
        }
        /// <summary>
        /// Returns a specified number of contiguous elements from the sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The sequence to return elements from</param>
        /// <param name="page">Current Page</param>
        /// <param name="pageSize">Number of items to take per page</param>
        /// <returns>
        /// An IQueryable that contains a selected number of elements from the source.
        /// </returns>
        public static IQueryable<T> SetPageData<T>(this IQueryable<T> query, int page, int pageSize)
        {
            int skip = (page) * pageSize;
            return query.Skip(skip).Take(pageSize);
        }
        /// <summary>
        /// Return the total number of pages for the selected Page Size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The sequence where the elements are from</param>
        /// <param name="pageSize">Number of items to take per page</param>
        /// <returns>The Number of Pages</returns>
        public static int GetTotalPages<T>(this IQueryable<T> query, int pageSize)
        {
            int count = query.Count();
            int pagesNumber = (count + pageSize) / pageSize;
            return pagesNumber;
        }
    }
}