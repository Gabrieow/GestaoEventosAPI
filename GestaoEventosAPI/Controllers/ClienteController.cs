using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GestaoEventosAPI.Application;
using GestaoEventosAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using GestaoEventosAPI.Application.DTOs;
using System.Security.Claims;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // exige autenticação para todos os métodos
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _key;

        public ClienteController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _key = jwtSettings.Value.SecretKey;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _context.Usuarios
                .Where(u => u.Role == Roles.Cliente)
                .ToListAsync();
            return Ok(clientes);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Cliente")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClienteUpdate dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            var cliente = await _context.Clientes.FindAsync(id);

            if (usuario == null || usuario.Role != Roles.Cliente || cliente == null)
                return NotFound();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
                return Unauthorized();

            if (userRole == Roles.Cliente.ToString() && userIdString != id.ToString())
                return Forbid();

            // Atualizar dados do usuário
            usuario.Nome = dto.Nome ?? usuario.Nome;
            usuario.Email = dto.Email ?? usuario.Email;

            // Atualizar dados do cliente
            cliente.Nome = dto.Nome ?? cliente.Nome;
            cliente.Email = dto.Email ?? cliente.Email;
            cliente.Telefone = dto.Telefone ?? cliente.Telefone;
            cliente.CPF = dto.CPF ?? cliente.CPF;
            cliente.Endereco = dto.Endereco ?? cliente.Endereco;
            cliente.Cidade = dto.Cidade ?? cliente.Cidade;
            cliente.Estado = dto.Estado ?? cliente.Estado;
            cliente.CEP = dto.CEP ?? cliente.CEP;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
