using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class PurchaseDataVM
    {
        public int Id { get; set; }

        public int? PurchaseOrderId { get; set; }
        public int? PurchaseId { get; set; }
        public string? PurchaseOrderCode { get; set; }
        public string? PurchaseCode { get; set; }

        public string? Code { get; set; }

        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }

        public string? Comments { get; set; }

        // 🔥 Financial Fields
        public decimal SubTotal { get; set; }
        public decimal TotalSD { get; set; }
        public decimal TotalVAT { get; set; }

        public decimal GrandTotal { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal PaidAmount { get; set; }

        public decimal DueAmount { get; set; }
        public decimal PaymentAmount { get; set; }

        public DateTime? InvoiceDateTime { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public PeramModel PeramModel { get; set; }

        public PurchaseDataVM()
        {
            PeramModel = new PeramModel();
        }
    }
}
