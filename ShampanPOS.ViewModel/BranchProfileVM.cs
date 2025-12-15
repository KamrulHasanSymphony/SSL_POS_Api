using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class BranchProfileVM
    {
        public int Id { get; set; }


        [Display(Name = "Code")]
        public string? Code { get; set; }


        [Display(Name = "Distributor Code")]
        public string? DistributorCode { get; set; }


        [Display(Name = "Distribution Name")]
        public string Name { get; set; }

        [Display(Name = "Bangla Name")]
        public string? BanglaName { get; set; }


        [Display(Name = "Area")]
        public int? AreaId { get; set; }

        [Display(Name = "Area Name")]
        public string? AreaName { get; set; }

        [Display(Name = "Telephone Number")]
        public string? TelephoneNo { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "VAT Registration Number")]
        public string? VATRegistrationNo { get; set; }

        [Display(Name = "BIN")]
        public string? BIN { get; set; }

        [Display(Name = "TIN Number")]
        public string? TINNO { get; set; }

        [Display(Name = "Comments")]
        public string? Comments { get; set; }

        [Display(Name = "Archived")]
        public bool IsArchive { get; set; }


        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        public string? CreatedFrom { get; set; }

        [Display(Name = "Created On")]

        public string? CreatedOn { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }
        public string? LastUpdateFrom { get; set; }

        [Display(Name = "Last Modified On")]

        public string? LastModifiedOn { get; set; }

        public string? Operation { get; set; }

        public string? Status { get; set; }
        public string? UserId { get; set; }
    }

}
