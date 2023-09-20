namespace Application.Features.Auth.DTOs
{
    public class RefreshTokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Expires { get; set; }
    }
}
