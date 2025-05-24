using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using GestaoEventosAPI.Data;
using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Application.DTOs;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Application;
using Microsoft.Extensions.Options;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly string _key;

    public AuthController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _key = jwtSettings.Value.SecretKey;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel register)
    {
        if (string.IsNullOrEmpty(register.Nome) || string.IsNullOrEmpty(register.Email) || string.IsNullOrEmpty(register.Senha))
        {
            return BadRequest("Todos os campos são obrigatórios.");
        }

        if (_context.Usuarios.Any(u => u.Email == register.Email))
            return Conflict("Já existe um usuário com este e-mail.");

        var usuario = new Usuario
        {
            Nome = register.Nome,
            Email = register.Email,
            SenhaHash = register.Senha
                .GetHashCode().ToString(), // simples hash para exemplo, usar método de hash seguro em produção
            Role = Roles.Cliente // definindo o papel como "Cliente" por padrão, para criar novos usuários admin ou organizadores, implementar direto no bd
        };
        
        // Adiciona o usuário ao banco de dados
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        return Ok("Usuário registrado com sucesso.");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Senha))
        {
            return BadRequest("Todos os campos são obrigatórios.");
        }

        var hashedSenha = login.Senha.GetHashCode().ToString();
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.SenhaHash == hashedSenha);

        if (usuario == null)
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        var token = GenerateToken(usuario.Email, usuario.Role);
        return Ok(new {
            token,
            nome = usuario.Nome,
            email = usuario.Email,
            role = usuario.Role.ToString()
        });
    }

    private string GenerateToken(string email, Roles role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(_key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
