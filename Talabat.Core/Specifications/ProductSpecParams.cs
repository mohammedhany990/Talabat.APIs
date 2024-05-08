using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications
{
    public class ProductSpecParams
    {
        
        public string? Sort { get; set; }
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }

        private int pagesize=5;

        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value >10 ? 10 : value; }
        }
        public int PageIndex { get; set; } = 1;

        public bool IsPagination { get; set; }

        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value.ToLower(); }
        }

    }
}
