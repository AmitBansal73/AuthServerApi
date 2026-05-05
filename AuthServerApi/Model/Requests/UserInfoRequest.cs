namespace AuthServerApi.Model.Requests
{
    public class UserInfoRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string DOB { get; set; } = string.Empty;
    }
}
