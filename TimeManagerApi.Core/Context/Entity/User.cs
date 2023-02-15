namespace TimeManagerApi.Core.Context.Entity;

public class User : BaseEntity
{
    public string UserName { get; set; } = null!;
    public string SecretWord { get; set; } = null!;
    public string Password { get; set; } = null!;
}