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
        if(String.IsNullOrWhiteSpace(model.Email))
            return Results.Json(StandartResponseAnswer.Error("Email обязателен для регистрации"));
        
        var newUser = new User()
        {
            UserName = model.UserName,
            Password = model.Password,
            Email = model.Email
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
            
        return Results.Json(StandartResponseAnswer.Ok<AuthorizationResponse>(new()
        {
            Token = GenerateToken(newUser),
            RefreshToken = GenerateToken(newUser, true)
        }));
    }

    [HttpGet("refresh")]
    public async Task<IResult> UpdateToken(string refreshToken)
    {
        string defaultSecureError = "Не удалось получить данных из токена";
        if(String.IsNullOrWhiteSpace(refreshToken))
            return Results.Json(StandartResponseAnswer.Error("Передан пустой токен"));
        
        var jsonToken = (new JwtSecurityTokenHandler().ReadToken(refreshToken)) as JwtSecurityToken;
        if (jsonToken == null)
            return Results.Json(StandartResponseAnswer.Error(defaultSecureError));
        
        Claim? user = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (user == null)
            return Results.Json(StandartResponseAnswer.Error(defaultSecureError));
        if (Guid.TryParse(user.Value, out Guid userId))
        {
            var needUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (needUser == null) 
                return Results.Json(StandartResponseAnswer.Error(defaultSecureError));
            
            return Results.Json(StandartResponseAnswer.Ok<AuthorizationResponse>(new()
            {
                Token = GenerateToken(needUser),
                RefreshToken = GenerateToken(needUser, true)
            }));
        }
        return Results.Json(StandartResponseAnswer.Error(defaultSecureError));
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