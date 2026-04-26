using System.ComponentModel.DataAnnotations;

namespace AuthServerApi.Model.Responses
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string PictureUrl { get; set; }

        public string MobileNumber { get; set; }
        public DateOnly Dob { get; set; }
    }
}
