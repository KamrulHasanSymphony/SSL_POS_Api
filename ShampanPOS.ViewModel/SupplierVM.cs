using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class SupplierVM
    {
        public int Id { get; set; }
        [Display(Name = "Code")]
        public string? Code { get; set; }
        [Display(Name = "Name")]
        public string? Name { get; set; }

        public int? MasterSupplierId { get; set; }
        public int? MasterSupplierGroupId { get; set; }
        public string? MasterSupplierGroupName { get; set; }

        [Display(Name = "Supplier Group ID")]
        public int? SupplierGroupId { get; set; }
        public string? SupplierGroupName { get; set; }
        public string? GroupName { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Telephone No")]
        public string? TelephoneNo { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Display(Name = "Comments")]
        public string? Comments { get; set; }

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
        public string? ImagePath { get; set; }

        public PeramModel PeramModel { get; set; }

        public List<SupplierVM> MasterSupplierList { get; set; }

        public SupplierVM()
        {
            MasterSupplierList = new List<SupplierVM>();
            PeramModel = new PeramModel();
        }

    }

}
