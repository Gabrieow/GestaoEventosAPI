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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null || cliente.Role != Roles.Cliente)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // só Admin pode criar
        public async Task<IActionResult> Create([FromBody] Usuario cliente)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            cliente.Role = Roles.Cliente;

            await _context.Usuarios.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // só Admin pode atualizar
        public async Task<IActionResult> Update(Guid id, [FromBody] Usuario cliente)
        {
            if (id != cliente.Id)
                return BadRequest();

            var existingCliente = await _context.Usuarios.FindAsync(id);
            if (existingCliente == null || existingCliente.Role != Roles.Cliente)
                return NotFound();

            existingCliente.Nome = cliente.Nome;
            existingCliente.Email = cliente.Email;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // só Admin pode deletar
        public async Task<IActionResult> Delete(Guid id)
        {
            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null || cliente.Role != Roles.Cliente)
                return NotFound();

            _context.Usuarios.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
