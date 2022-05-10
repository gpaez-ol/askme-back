using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsKMe.Pagination
{
    public class PaginationResult<T>
    {
        /// <Summary>
        /// Page Number
        /// </Summary>
        public int PageNumber { get; set; }
        /// <Summary>
        /// Total Elements
        /// </Summary>

        public int TotalElements { get; set; }
        /// <Summary>
        /// Total Pages
        /// </Summary>
        public int TotalPages { get; set; }

        /// <Summary>
        /// List of items
        /// </Summary>
        public ICollection<T> Content { get; set; }

    }
}
