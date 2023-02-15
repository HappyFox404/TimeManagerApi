namespace TimeManagerApi.Models.Requests.Authorization;

public class RegistrationModel : AuthorizationModel
{
    public string SecretWord { get; set; } = null!;
}