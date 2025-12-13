using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShampanPOS.ViewModel
{
    public class ProductReplaceReceiveVM
    {
        public int Id { get; set; }
        public int ProductReplaceReceiveId { get; set; }
        [Display(Name = "Code")]
        public string? Code { get; set; }
        public string ReceiveDate { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public string? BranchName { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }

        [Display(Name = "Created From")]
        public string? CreatedFrom { get; set; }

        [Display(Name = "Last Update From")]
        public string? LastUpdateFrom { get; set; }
        [Display(Name = "Posted")]
        public bool IsPost { get; set; }
        public string? Status { get; set; }

        [Display(Name = "Posted By")]
        public string? PostedBy { get; set; }

        [Display(Name = "Posted On")]
        public string? PostedOn { get; set; }

        public List<ProductReplaceReceiveDetailsVM> ProductReplaceReceiveDetails { get; set; }

        public ProductReplaceReceiveVM()
        {
            ProductReplaceReceiveDetails = new List<ProductReplaceReceiveDetailsVM>();
        }
    }

}
