using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TimeManagerApi.Models.Settings;

public class TokenSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    
    public SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}