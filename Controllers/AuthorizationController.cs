using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TimeManagerApi.Core.Context;
using TimeManagerApi.Core.Context.Entity;
using TimeManagerApi.Models;
using TimeManagerApi.Models.Requests;
using TimeManagerApi.Models.Requests.Authorization;
using TimeManagerApi.Models.Responses;
using TimeManagerApi.Models.Settings;

namespace TimeManagerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly ILogger<AuthorizationController> _logger;
    private readonly TimeManagerContext _context;
    private readonly TokenSettings _tokenSettings;
    
    public AuthorizationController(ILogger<AuthorizationController> logger, TimeManagerContext context, IOptions<TokenSettings> tokenSettings)
    {
        _logger = logger;
        _context = context;
        _tokenSettings = tokenSettings.Value;
    }
    
    [HttpGet]
    public async Task<IResult> Get(AuthorizationModel model)
    {
        var needUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName
        && x.Password == model.Password);
        if (needUser != null)
        {
            return Results.Json(StandartResponseAnswer.Ok<AuthorizationResponse>(new()
            {
                Token = GenerateToken(needUser),
                RefreshToken = GenerateToken(needUser,true)
            }));
        }
        return Results.Json(StandartResponseAnswer.Error("Не найден пользователь"));
    }
    
    [HttpPost("register")]
    public async Task<IResult> Post(RegistrationModel model)
    {
        var needUser = await _context.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
        if (needUser != null)
            return Results.Json(StandartResponseAnswer.Error("Пользователь с таким именем уже существует"));
        if(String.IsNullOrWhiteSpace(model.SecretWord))
            return Results.Json(StandartResponseAnswer.Error("ВЫ не задали секретное слово для восстановления"));
        
        var newUser = new User()
        {
            UserName = model.UserName,
            Password = model.Password,
            SecretWord = model.SecretWord
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
            
        return Results.Json(StandartResponseAnswer.Ok<AuthorizationResponse>(new()
        {
            Token = GenerateToken(newUser),
            RefreshToken = GenerateToken(newUser, true)
        }));
        
    }

    private string GenerateToken(User user, bool isRefresh = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };
        
        var jwt = new JwtSecurityToken(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes((isRefresh == false) ? 5 : 10)),
            signingCredentials: new SigningCredentials(_tokenSettings.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}