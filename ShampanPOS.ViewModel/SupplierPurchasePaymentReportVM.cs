using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SupplierPurchasePaymentReportVM
    {
        public string? Operation { get; set; }
        public bool IsSummary { get; set; }

        // Filters
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        // Summary fields
        public int? PurchaseCount { get; set; }
        public decimal? TotalPurchaseAmount { get; set; }
        public int? PaymentCount { get; set; }
        public decimal? TotalPaymentAmount { get; set; }
        public decimal? OutstandingAmount { get; set; }

        // Details fields
        public string? TransactionType { get; set; }
        public int? PurchaseId { get; set; }
        public string? PurchaseCode { get; set; }
        public string? PurchaseDate { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public int? PaymentId { get; set; }
        public string? PaymentCode { get; set; }
        public string? PaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }

        // Common
        public int? SupplierIdResult { get; set; }
        public string? SupplierCode { get; set; }
        public string? SupplierNameResult { get; set; }
    }
}
