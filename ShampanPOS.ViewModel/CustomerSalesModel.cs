using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class CustomerSalesModel
    {
        public int? CustomerId { get; set; }
        public int? BranchId { get; set; }
        public string? CustomerName { get; set; }
        public decimal? TotalGrandTotalAmount { get; set; }
        public int? TotalQuantity { get; set; }
    }
}
