using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SaleOrderStatusVM
    {
        // Filter params
        public int? BranchId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public int? SaleOrderId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? StatusFilter { get; set; } // "All", "Completed", "Remaining"

        // Report output — Order level
        public int? OrderId { get; set; }
        public string? OrderCode { get; set; }
        public string? OrderDate { get; set; }
        public string? DeliveryDate { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }

        // Report output — Detail level
        public int? DetailId { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public decimal? OrderedQty { get; set; }
        public decimal? CompletedQty { get; set; }
        public decimal? RemainQty { get; set; }
        public decimal? UnitRate { get; set; }
        public decimal? LineTotal { get; set; }
        public bool IsCompleted { get; set; }

        // Header info
        public string? BranchName { get; set; }
        public string? BranchAddress { get; set; }
        public string? CompanyName { get; set; }
    }
}
