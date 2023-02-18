using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using TimeManagerApi.Core.Context;
using TimeManagerApi.Models.Settings;
using TimeManagerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSetings"));
builder.Services.AddDbContext<TimeManagerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("TokenSetings:Issuer"),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("TokenSetings:Audience"),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetValue<string>("TokenSetings:Key"))),
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddControllers();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthorization();

app.MapControllers();

app.Run();