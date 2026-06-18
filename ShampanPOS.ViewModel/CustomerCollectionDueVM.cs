using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class CustomerCollectionDueVM
    {
        public int? CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchAddress { get; set; }
        public string? CompanyName { get; set; }
        public decimal? TotalSaleAmount { get; set; }
        public int? SaleCount { get; set; }
        public decimal? TotalCollectionAmount { get; set; }
        public int? CollectionCount { get; set; }
        public decimal? DueAmount { get; set; }
    }
}
