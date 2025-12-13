using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class SalesPersonVM
    {
        public int Id { get; set; }
        [Display(Name = "Code")]
        public string? Code { get; set; }
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Display(Name = "Branch ID")]
        public int? BranchId { get; set; }
        [Display(Name = "Parent ID")]
        public int? ParentId { get; set; }
        [Display(Name = "Enum Type ID")]
        public int? EnumTypeId { get; set; }
        public string? Type { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }

        [Display(Name = "Comments")]
        public string? Comments { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Fax No")]
        public string? FaxNo { get; set; }

        [Display(Name = "NID No")]
        public string? NIDNo { get; set; }
        [Display(Name = "Mobile")]
        public string? Mobile { get; set; }
        [Display(Name = "Mobile 2")]
        public string? Mobile2 { get; set; }
        [Display(Name = "Phone")]
        public string? Phone { get; set; }
        [Display(Name = "Phone 2")]
        public string? Phone2 { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address 2")]
        public string? EmailAddress2 { get; set; }

        [Display(Name = "Fax")]
        public string? Fax { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }
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

        public string Operation { get; set; }
        public string? Status { get; set; }
        public string? ImagePath { get; set; }

    }

}
