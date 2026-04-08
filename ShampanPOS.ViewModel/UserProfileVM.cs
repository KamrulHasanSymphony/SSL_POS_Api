using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class UserProfileVM
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Display(Name = "Phone Number")]

        [Required(ErrorMessage = "Please enter your phone number.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Please enter a valid 11-digit phone number.")]
        public string PhoneNumber { get; set; }
        public string? NormalizedPassword { get; set; }
        public string? Operation { get; set; }
        public string? Mode { get; set; }
        public bool IsAdmin { get; set; }
        public int? SalePersonId { get; set; }
        public bool IsSalePerson { get; set; }
        public bool IsHeadOffice { get; set; }
        public string? SalePersonName { get; set; }
        public string? SalePersonCode { get; set; }


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
        public string? ImagePath { get; set; }

    }

    public static class DefaultRoles
    {
        public const string Role = "User";
    }

}
