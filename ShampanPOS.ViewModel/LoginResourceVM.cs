using System.ComponentModel.DataAnnotations;

namespace ShampanPOS.ViewModel
{

    public class LoginResourceVM
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "MIN_PASSWORD_LENGTH")]
        public string Password { get; set; }

        public string? Message { get; set; }
        public bool RememberMe { get; set; }
        public bool shouldLockout { get; set; }

        public string? returnUrl { get; set; }
        public string? dbName { get; set; }

        
        public int? CompanyId { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyDatabase { get; set; }

    }

    public class AuthModel
    {
        public string token { get; set; }

        public string Token_type { get; set; }

        public string Expires_in { get; set; }
    }

}
