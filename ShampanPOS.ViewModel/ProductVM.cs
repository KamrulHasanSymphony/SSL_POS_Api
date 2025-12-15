using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{
    public class ProductVM
    {
        public int Id { get; set; }

        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Product Group")]
        public int? ProductGroupId { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "UOM")]
        public int? UOMId { get; set; }

        [Display(Name = "HS Code No")]
        public string? HSCodeNo { get; set; }

        public decimal? VATRate { get; set; }
        public decimal? SDRate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }


        [Display(Name = "Archived")]
        public bool IsArchive { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }
        public string? CreatedFrom { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }
        public string? LastUpdateFrom { get; set; }

        public string? Operation { get; set; }
        public string? Status { get; set; }
        public string? ProductGroupName { get; set; }
        public string? UOMName { get; set; }
        public string? ImagePath { get; set; }
     
        public List<BranchProfileVM> BranchProfileList { get; set; }

        public ProductVM()
        {
            BranchProfileList = new List<BranchProfileVM>();
        }

    }


}
