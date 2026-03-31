using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SaleOrderStatusModelVM
    {
        public string SaleOrderId { get; set; }
        public decimal CompletedQty { get; set; }
        public decimal PendingQty { get; set; }
    }
}
