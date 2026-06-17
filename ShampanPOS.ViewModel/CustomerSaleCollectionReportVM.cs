using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class CustomerSaleCollectionReportVM
    {
        public int? CustomerId { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public int? SaleId { get; set; }
        public string? SaleCode { get; set; }
        public string? InvoiceDate { get; set; }
        public decimal? SaleAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public int? CollectionId { get; set; }
        public string? CollectionCode { get; set; }
        public string? CollectionDate { get; set; }
        public decimal? CollectionAmount { get; set; }
        public string? TransactionType { get; set; }
        public bool IsSummary { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Operation { get; set; }

        // Summary fields
        public int? SaleCount { get; set; }
        public int? CollectionCount { get; set; }
        public decimal? TotalSaleAmount { get; set; }
        public decimal? TotalCollectionAmount { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public string? LastCollectionDate { get; set; }
        public decimal? LastCollectionAmount { get; set; }
    }

}
