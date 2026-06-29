using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ShampanPOS.ViewModel
{

    public class LoginResourceVM
    {

        [Required(ErrorMessage = "User Name is required")]
        [DisplayName("User Name")]

        // Only letters allowed (A-Z, a-z)
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "User Name can contain only letters (A-Z, a-z)")]
        public string UserName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters long.")]
        public string Password { get; set; }

        public string? Message { get; set; }
        public string? Id { get; set; }
        public bool RememberMe { get; set; }
        public bool shouldLockout { get; set; }

        public string? returnUrl { get; set; }
        public string? dbName { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyDatabase { get; set; }

        public List<CompanyInfo> CompanyInfos { get; set; }
        public List<UserProfileVM> UserInfos { get; set; }

        public LoginResourceVM()
        {
            CompanyInfos = new List<CompanyInfo>();
            UserInfos = new List<UserProfileVM>();
        }



    }

    public class AuthModel
    {
        public string token { get; set; }

        public string Token_type { get; set; }

        public string Expires_in { get; set; }
    }

    public class CompanyInfo
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDataBase { get; set; }
        public int SerialNo { get; set; }
        public string DatabaseName { get; set; }
    }

}
