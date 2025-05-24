using GestaoEventosAPI.Data;
using GestaoEventosAPI.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================================

// Configuração de autenticação JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
{
    throw new InvalidOperationException("JWT settings or SecretKey is not configured properly.");
}
var key = jwtSettings.SecretKey;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

// ===============================================

// connection string SQLite
var connectionString = "Data Source=gestaoeventos.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// ===============================================

// Configuração de serviços

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.MapControllers();

app.Run();