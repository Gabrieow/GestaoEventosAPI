using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Data;
using GestaoEventosAPI.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _key;

        public EventoController(AppDbContext context, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _key = jwtSettings.Value.SecretKey;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventos = await _context.Eventos.ToListAsync();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            return Ok(evento);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Create([FromBody] Evento evento)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var organizador = await _context.Usuarios.FindAsync(Guid.Parse(userId));
            if (organizador == null || organizador.Role != Roles.Organizador)
                return Forbid();

            evento.OrganizadorId = organizador.Id;
            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = evento.Id }, evento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Evento evento)
        {
            if (id != evento.Id)
                return BadRequest();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var organizador = await _context.Usuarios.FindAsync(Guid.Parse(userId));
            if (organizador == null || organizador.Role != Roles.Organizador)
                return Forbid();

            evento.OrganizadorId = organizador.Id;
            _context.Entry(evento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var organizador = await _context.Usuarios.FindAsync(Guid.Parse(userId));
            if (organizador == null || organizador.Role != Roles.Organizador)
                return Forbid();

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var eventos = await _context.Eventos
                .Where(e => e.Nome.Contains(query) || e.Descricao.Contains(query))
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(string categoria, DateTime? dataInicio, DateTime? dataFim)
        {
            var eventos = _context.Eventos.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
                eventos = eventos.Where(e => e.Categoria == categoria);

            if (dataInicio.HasValue)
                eventos = eventos.Where(e => e.DataInicio >= dataInicio.Value);

            if (dataFim.HasValue)
                eventos = eventos.Where(e => e.DataFim <= dataFim.Value);

            return Ok(await eventos.ToListAsync());
        }

        [HttpGet("organizador/{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> GetByOrganizador(Guid id)
        {
            var eventos = await _context.Eventos.Where(e => e.OrganizadorId == id).ToListAsync();

            if (eventos == null || !eventos.Any())
                return NotFound();

            return Ok(eventos);
        }

        [HttpGet("cliente/{id}")]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> GetByCliente(Guid id)
        {
            var cliente = await _context.Usuarios.FindAsync(id);
            if (cliente == null || cliente.Role != Roles.Cliente)
                return NotFound();

            var ingressos = await _context.Ingressos
                .Include(i => i.Evento)
                .Where(i => i.ClienteId == id)
                .ToListAsync();

            var eventos = ingressos.Select(i => i.Evento).ToList();

            return Ok(eventos);
        }
    }
}
