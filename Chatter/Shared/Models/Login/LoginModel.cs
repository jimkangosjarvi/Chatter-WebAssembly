using System.ComponentModel.DataAnnotations;

namespace Chatter.Shared.Models.Login
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; }


        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
