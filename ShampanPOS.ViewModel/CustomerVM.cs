using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class CustomerVM
    {
        public int Id { get; set; }
        public int? SL { get; set; }


        [Display(Name = "Code")]
        public string? Code { get; set; }


        [Display(Name = "Name")]
        public string? Name { get; set; }


        [Display(Name = "Branch ID")]
        public int? BranchId { get; set; }

        [Display(Name = "Focal Point")]
        public int? FocalPointId { get; set; }
        [Display(Name = "Customer Group ID")]
        public int? CustomerGroupId { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }
        [Display(Name = "Customer Category")]
        public string? CustomerCategory { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Bangla Address")]
        public string? BanglaAddress { get; set; }


        [Display(Name = "Route ID")]
        public int? RouteId { get; set; }


        [Display(Name = "Area ID")]
        public int? AreaId { get; set; }

        [Display(Name = "City")]
        public string? City { get; set; }

        [Display(Name = "Telephone No")]
        public string? TelephoneNo { get; set; }

        [Display(Name = "Fax No")]
        public string? FaxNo { get; set; }


        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "TIN No")]
        public string? TINNo { get; set; }

        [Display(Name = "BIN No")]
        public string? BINNo { get; set; }

        [Display(Name = "NID No")]
        public string? NIDNo { get; set; }

        [Display(Name = "Comments")]
        public string? Comments { get; set; }


        [Display(Name = "Archived")]
        public bool IsArchive { get; set; }


        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }


        [Display(Name = "Created On")]
        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Last Modified On")]
        public string? LastModifiedOn { get; set; }

        [Display(Name = "Operation")]
        public string? Operation { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "CreatedFrom")]
        public string? CreatedFrom { get; set; }

        [Display(Name = "Last Update From")]
        public string? LastUpdateFrom { get; set; }
        public string? Status { get; set; }
        public string? ImagePath { get; set; }

        public string? CustomerGroupName { get; set; }


    }


}
