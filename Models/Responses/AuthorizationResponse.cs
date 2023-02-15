namespace TimeManagerApi.Models.Responses;

public class AuthorizationResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}