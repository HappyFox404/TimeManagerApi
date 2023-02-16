namespace TimeManagerApi.Models.Requests.Authorization;

public class RegistrationModel : AuthorizationModel
{
    public string Email { get; set; } = null!;
}