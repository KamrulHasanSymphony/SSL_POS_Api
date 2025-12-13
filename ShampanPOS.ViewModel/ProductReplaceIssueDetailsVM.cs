using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class ProductReplaceIssueDetailsVM
    {
        public int Id { get; set; }
        public int ProductReplaceIssueId { get; set; }
        public int? ProductReplaceReceiveDetailId { get; set; }
        public int? ProductReplaceReceiveId { get; set; }


        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? UOM { get; set; }
        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }
        public int? Line { get; set; }
        public string? ReceiveCode { get; set; }

    }

}
