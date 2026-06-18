using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SupplierPaymentDueVM
    {
        public int? SupplierId { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierName { get; set; }
        public string? BranchCode { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public int? PurchaseCount { get; set; }
        public decimal? TotalPaymentAmount { get; set; }
        public int? PaymentCount { get; set; }
        public decimal? LastPaymentAmount { get; set; }
        public decimal? DueAmount { get; set; }
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
    }
}
