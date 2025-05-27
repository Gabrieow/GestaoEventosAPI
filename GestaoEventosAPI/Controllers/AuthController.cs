using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

using GestaoEventosAPI.Data;
using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Application.DTOs;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Application;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _secretKey;

        public AuthController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _secretKey = jwtSettings.Value.SecretKey;
            if (string.IsNullOrEmpty(_secretKey))
                throw new Exception("Chave secreta JWT não configurada no appsettings.json");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuarioExistente != null)
                return BadRequest("Email já cadastrado.");

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = HashGenerator.ComputeSha256Hash(dto.Senha),
                Role = Roles.Cliente // definindo como cliente direto
            };

            var cliente = new Cliente
            {
                Id = usuario.Id, // mesmo Id do usuário para vincular
                Nome = dto.Nome,
                Email = dto.Email,

                Telefone = "",
                CPF = "",
                Endereco = "",
                Cidade = "",
                Estado = "",
                CEP = ""
            };

            _context.Usuarios.Add(usuario);
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { usuario.Id, dto.Nome, dto.Email });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Senha))
                return BadRequest("Todos os campos são obrigatórios.");

            var hashedSenha = HashGenerator.ComputeSha256Hash(login.Senha);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.SenhaHash == hashedSenha);

            if (usuario == null)
                return Unauthorized("E-mail ou senha inválidos.");

            var token = GenerateToken(usuario.Email, usuario.Role, usuario.Id);

            return Ok(new
            {
                token,
                nome = usuario.Nome,
                email = usuario.Email,
                role = usuario.Role.ToString()
            });
        }

        [HttpGet("usuarios")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    Role = u.Role.ToString()
                })
                .ToList();

            return Ok(usuarios);
        }

        private string GenerateToken(string email, Roles role, Guid userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "seuIssuer",
                audience: "seuAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
            }
}
