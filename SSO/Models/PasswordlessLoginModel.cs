using System.ComponentModel.DataAnnotations;

namespace IDPExample.SSO.Models
{
    public class PasswordlessLoginModel
    {
        public string Email { get; set; }
        public string ReturnUrl { get; set; }
        public string Error { get; set; }
        public string Success { get; set; }
        public string Link { get; set; }
    }
}