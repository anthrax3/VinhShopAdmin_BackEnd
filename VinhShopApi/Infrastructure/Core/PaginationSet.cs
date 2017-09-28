using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VinhShopApi.Infrastructure.Core
{
    public class PaginationSet<T>
    {
        public int PageIndex { set; get; }
        public int PageSize { get; set; }
        public int TotalRows { set; get; }
        public IEnumerable<T> Items { set; get; }
    }
}