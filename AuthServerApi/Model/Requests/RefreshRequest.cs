using System.ComponentModel.DataAnnotations;

namespace AuthServerApi.Model.Requests
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
