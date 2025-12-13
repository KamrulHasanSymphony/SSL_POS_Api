using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class ContactPersonVM
    {
        public int Id { get; set; }


        [Display(Name = "Name")]
        public string? Name { get; set; }


        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Display(Name = "Designation")]
        public string? Designation { get; set; }


        [Display(Name = "Mobile")]
        public string? Mobile { get; set; }

        [Display(Name = "Alternate Mobile No.")]
        public string? Mobile2 { get; set; }


        [Display(Name = "Phone")]
        public string? Phone { get; set; }


        [Display(Name = "Alternate Phone No.")]
        public string? Phone2 { get; set; }

        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }


        [Display(Name = "Alternate Email Address")]
        public string? EmailAddress2 { get; set; }

        [Display(Name = "Fax")]
        public string? Fax { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }


        [Display(Name = "Country ID")]
        public int? CountryId { get; set; }

        [Display(Name = "Division ID")]
        public int? DivisionId { get; set; }


        [Display(Name = "District ID")]
        public int? DistrictId { get; set; }


        [Display(Name = "Thana ID")]
        public int? ThanaId { get; set; }


        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }

        [Display(Name = "Archived")]
        public bool? IsArchive { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "Operation")]
        public string? Operation { get; set; }


        [Display(Name = "Active Status")]
        public string? Status { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }


        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }

        [Display(Name = "Last Update From")]
        public string? LastUpdateFrom { get; set; }

        public string? CountryName { get; set; }
        public string? DivisionName { get; set; }
        public string? DistrictName { get; set; }
        public string? ThanaName { get; set; }


    }


}
