using System;
using System.Collections.Generic;
using SMS.Models;

namespace SMS.Web.Models
{
    public class StudentPagedViewModel
    {
        public IEnumerable<SMS.Models.Student> Students { get; set; } = new List<SMS.Models.Student>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
