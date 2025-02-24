using System.ComponentModel.DataAnnotations;

namespace AuthServerApi.Model
{
    public class User
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string HashPassword { get; set; }

        [Required]
        public string MobileNumber { get; set; }
        public DateOnly Dob { get; set; }

    }
}
