using System;
using System.Collections.Generic;

namespace IDM.DTO.Main
{ 
        public class PagedResultDTO<T> : DocumentAuditDTO
        {
            public List<T> Items { get; set; } = new List<T>();
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }

            public bool HasPreviousPage { get { return CurrentPage > 1; } } 
            public bool HasNextPage { get { return CurrentPage < TotalPages; } }
        } 
}
