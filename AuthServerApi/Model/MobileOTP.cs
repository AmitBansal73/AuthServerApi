using System.ComponentModel.DataAnnotations;

namespace AuthServerApi.Model
{
    public class MobileOTP
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string OTP { get; set; }

        [Required]
        public DateTime CreatedAt {  get; set; }
    }
}
