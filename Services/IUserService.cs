using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TimeManagerApi.Core.Context;
using TimeManagerApi.Core.Context.Entity;

namespace TimeManagerApi.Services;

public interface IUserService
{
    Task<User> GetCurrentUser();
    Task<Guid> GetCurrentUserId();
}

public class UserService : IUserService
{
    private readonly TimeManagerContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IHttpContextAccessor _httpAccesor;
    
    public UserService(TimeManagerContext context, ILogger<UserService> logger, IHttpContextAccessor httpAccesor)
    {
        _context = context;
        _logger = logger;
        _httpAccesor = httpAccesor;
    }
    
    public async Task<User> GetCurrentUser()
    {
        var needClaim = _httpAccesor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (needClaim != null)
        {
            if (Guid.TryParse(needClaim.Value, out Guid userId) == false)
                throw new ArgumentException("Не удалось получить идентификатор пользователя");

            var needUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (needUser == null)
                throw new ArgumentNullException("Пользователь не найден");
            return needUser;
        }
        throw new AuthenticationException("Пользователь не авторизирован в api");
    }
    
    public async Task<Guid> GetCurrentUserId()
    {
        var needClaim = _httpAccesor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        if (needClaim != null)
        {
            if (Guid.TryParse(needClaim.Value, out Guid userId) == false)
                throw new ArgumentException("Не удалось получить идентификатор пользователя");

            var needUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (needUser == null)
                throw new ArgumentNullException("Пользователь не найден");
            return needUser.Id;
        }
        throw new AuthenticationException("Пользователь не авторизирован в api");
    }
}