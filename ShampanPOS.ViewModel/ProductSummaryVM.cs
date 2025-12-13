using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class ProductSummaryVM
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string EffectDate { get; set; }
        public string GroupName { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal CostPrice { get; set; }
    }
}
