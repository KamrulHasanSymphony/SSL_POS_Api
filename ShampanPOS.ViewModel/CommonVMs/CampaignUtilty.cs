using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel.CommonVMs
{
    public class CampaignUtilty
    {
        public int BranchId { get; set; }
        public string? Date { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public PeramModel PeramModel { get; set; }

        public CampaignUtilty()
        {
            PeramModel = new PeramModel();
        }

    }
}
