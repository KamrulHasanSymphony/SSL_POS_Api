using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class SaleDetailVM
    {
        public int Id { get; set; }
        public string? Code { get; set; }

        [Display(Name = "Sale ID")]
        public string? SaleId { get; set; }

        [Display(Name = "Sale Delivery ID")]
        public int? SaleDeliveryId { get; set; }

        [Display(Name = "Sale Delivery Detail ID")]
        public int? SaleDeliveryDetailId { get; set; }

        [Display(Name = "Sale Order ID")]
        public int? SaleOrderId { get; set; }

        [Display(Name = "Sale Order Detail ID")]
        public int? SaleOrderDetailId { get; set; }

        [Display(Name = "Branch ID")]
        public int? BranchId { get; set; }

        [Display(Name = "Line")]
        public int? Line { get; set; }
        public string? ProductName { get; set; }

        [Display(Name = "Product ID")]
        public int? ProductId { get; set; }

        [Display(Name = "Input Qty")]
        public decimal InputQuantity { get; set; }
        [Display(Name = "Input Qty")]
        public decimal CtnQuantity { get; set; }
        [Display(Name = "Quantity")]
        public decimal PcsQuantity { get; set; }

        [Display(Name = "Currency")]
        [DataType(DataType.Currency)]
        public decimal? Quantity { get; set; }

        [Display(Name = "Unit Rate")]
        [DataType(DataType.Currency)]
        public decimal? UnitRate { get; set; }

        [Display(Name = "Sub Total")]
        [DataType(DataType.Currency)]
        public decimal? SubTotal { get; set; }

        [Display(Name = "SD")]
        [DataType(DataType.Currency)]
        public decimal? SD { get; set; }

        [Display(Name = "SD Amount")]
        [DataType(DataType.Currency)]
        public decimal? SDAmount { get; set; }

        [Display(Name = "VAT Rate")]
        public decimal? VATRate { get; set; }

        [Display(Name = "VAT Amount")]
        [DataType(DataType.Currency)]
        public decimal? VATAmount { get; set; }

        [Display(Name = "Line Total")]
        [DataType(DataType.Currency)]
        public decimal? LineTotal { get; set; }
        public string? UOMName { get; set; }

        [Display(Name = "UOM ID")]
        public int? UOMId { get; set; }

        [Display(Name = "UOM From ID")]
        public int? UOMFromId { get; set; }
        public string? UOMFromName { get; set; }

        [Display(Name = "UOM Conversion")]
        public decimal? UOMConversion { get; set; }

        [Display(Name = "Comments")]
        public string? Comments { get; set; }

        [Display(Name = "VAT Type")]
        public string? VATType { get; set; }

        [Display(Name = "Transaction Type")]
        public string? TransactionType { get; set; }
        public string? Operation { get; set; }

        [Display(Name = "Posted")]
        public bool IsPost { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? CreatedFrom { get; set; }
        public string? LastModifiedBy { get; set; }
        public string? LastModifiedOn { get; set; }
        public string? LastUpdateFrom { get; set; }

        public int? CampaignTypeId { get; set; }
        public int? CampaignDetailsId { get; set; }
        public int? CampaignHeaderId { get; set; }


    }


}
