namespace AuthServerApi.Model.Responses
{
    public class AuthenticatedUserResponse
    {
        public string AuthToken { get; set; }

        public string RefreshToken { get; set; }

        public UserInfo UserInfo { get; set; }
    }
}
