namespace TimeManagerApi.Models.Requests.Authorization;

public class AuthorizationModel
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}