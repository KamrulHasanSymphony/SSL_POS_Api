using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class ProductStockSummary
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string OpeningDate { get; set; }
        public int OpeningQuantity { get; set; }
        public int OpeningValue { get; set; }
    }
}
