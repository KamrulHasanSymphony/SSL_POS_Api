using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using ShampanPOS.ViewModel.CommonVMs;

namespace ShampanPOS.ViewModel
{

public class CustomerPaymentCollectionVM
    {
       
        public int? Id { get; set; }
        [Display(Name = "Code")]
        public string? Code { get; set; }
        [Display(Name = "Customer ")]
        public int? CustomerId { get; set; }

        [Display(Name = "Transaction Date")]
        public string? TransactionDate { get; set; }
        [Display(Name = "Mode Of Payment")]
        public string? ModeOfPayment { get; set; }
        [Display(Name = "Mode Of Payment No")]
        public string? ModeOfPaymentNo { get; set; }
        public string? ModeOfPaymentDate { get; set; }
        [Display(Name = "Sale Person ")]
        public string? UserId { get; set; }
        public string? Attachment { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RestAmount { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }
        public string? Operation { get; set; }
        public string? Status { get; set; }
        public string? ModeOfPaymentName { get; set; }
        public string? CustomerName { get; set; }
        public bool IsPost { get; set; }
        public bool IsProcessed { get; set; }

        public string? ImagePath { get; set; }
        public string? FileName { get; set; }
        public int BranchId { get; set; }

    }


}
