using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GestaoEventosAPI.Application.DTOs;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IngressoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngressoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdString);

            IQueryable<Ingresso> query = _context.Ingressos
                .Include(i => i.Evento)
                .Include(i => i.Cliente);

            if (userRole == Roles.Cliente.ToString())
            {
                query = query.Where(i => i.ClienteId == userId);
            }

            var ingressos = await query.ToListAsync();
            return Ok(ingressos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ingresso = await _context.Ingressos
                .Include(i => i.Evento)
                .Include(i => i.Cliente)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (ingresso == null)
                return NotFound();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
                return Unauthorized();

            var userId = Guid.Parse(userIdString);

            // só permite se for admin, org ou dono do ingresso
            if (userRole == Roles.Admin.ToString() ||
                userRole == Roles.Organizador.ToString() ||
                ingresso.ClienteId == userId)
            {
                return Ok(ingresso);
            }

            return Forbid();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Create([FromBody] Ingresso ingresso)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            var evento = await _context.Eventos.FindAsync(ingresso.EventoId);
            if (evento == null)
                return BadRequest("Evento não encontrado.");

            ingresso.ClienteId = clienteId;

            _context.Ingressos.Add(ingresso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ingresso.Id }, ingresso);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Update(Guid id, [FromBody] IngressoUpdateDTO ingressoDto)
        {
            var existingIngresso = await _context.Ingressos.FindAsync(id);
            if (existingIngresso == null)
                return NotFound();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            if (userRole == Roles.Cliente.ToString() && existingIngresso.ClienteId != clienteId)
                return Forbid();

            existingIngresso.EventoId = ingressoDto.EventoId;
            existingIngresso.Quantidade = ingressoDto.Quantidade;
            existingIngresso.Preco = ingressoDto.Preco;
            existingIngresso.TipoIngresso = ingressoDto.TipoIngresso;

            _context.Entry(existingIngresso).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdString == null || userRole == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            var ingresso = await _context.Ingressos.FindAsync(id);
            if (ingresso == null)
                return NotFound();

            if (userRole == Roles.Cliente.ToString() && ingresso.ClienteId != clienteId)
                return Forbid();

            _context.Ingressos.Remove(ingresso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("cliente/{clienteId}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> GetByCliente(Guid clienteId)
        {
            var cliente = await _context.Usuarios.FindAsync(clienteId);
            if (cliente == null || cliente.Role != Roles.Cliente)
                return NotFound();

            var ingressos = await _context.Ingressos
                .Include(i => i.Evento)
                .Where(i => i.ClienteId == clienteId)
                .ToListAsync();

            return Ok(ingressos);
        }
    }
}
