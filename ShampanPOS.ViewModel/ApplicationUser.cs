using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string NormalizedName { get; set; }
        public string NormalizedUserName { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public string NormalizedEmail { get; set; }
        public int? SalePersonId { get; set; }
        public string? ImagePath { get; set; }
        public bool IsSalePerson { get; set; }
        public bool IsHeadOffice { get; set; }

        [StringLength(256)]
        public string NormalizedPassword { get; set; }

    }


}
