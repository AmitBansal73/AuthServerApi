using System.ComponentModel.DataAnnotations;

namespace AuthServerApi.Model.Requests
{
    public class RegistrationRequest
    {

        [Required]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string confPassword { get; set; }

        [Required]
        public string mobileNumber { get; set; }
        public DateOnly dob { get; set; }
    }
}
