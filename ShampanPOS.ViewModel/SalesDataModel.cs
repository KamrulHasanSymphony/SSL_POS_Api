using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SalesDataModel
    {
        public int SaleOrdered { get; set; }
        public int SaleDelivered { get; set; }
        public int SaleDeliveryReturn { get; set; }
        public int Sales { get; set; }
        public int SaleReturn { get; set; }
    }
}
