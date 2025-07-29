using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Core.Common
{
    public class QueryOptions : PaginationParameters
    {
        public string? Search { get; set; }
        public string SortField { get; set; } = "Date";
        public bool SortDescending { get; set; } = false;

        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }

        public string? Name { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
