using GestaoEventosAPI.Domain.Entities;
    using GestaoEventosAPI.Domain.Enums;
    using GestaoEventosAPI.Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Claims;

namespace GestaoEventosAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
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
            var ingressos = await _context.Ingressos
                .Include(i => i.Evento)
                .Include(i => i.Cliente)
                .ToListAsync();
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

            return Ok(ingresso);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Create([FromBody] Ingresso ingresso)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // pega o ID do cliente autenticado no token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            // valida se o evento existe
            var evento = await _context.Eventos.FindAsync(ingresso.EventoId);
            if (evento == null)
                return BadRequest("Evento não encontrado.");

            // associa o ingresso ao cliente autenticado (ignora o cliente enviado no corpo, se existir)
            ingresso.ClienteId = clienteId;

            _context.Ingressos.Add(ingresso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = ingresso.Id }, ingresso);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Ingresso ingresso)
        {
            if (id != ingresso.Id)
                return BadRequest();

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            var existingIngresso = await _context.Ingressos.FindAsync(id);
            if (existingIngresso == null)
                return NotFound();

            // verifica se o ingresso pertence ao cliente logado
            if (existingIngresso.ClienteId != clienteId)
                return Forbid();

            existingIngresso.Quantidade = ingresso.Quantidade;

            _context.Entry(existingIngresso).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Cliente")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
                return Unauthorized();

            var clienteId = Guid.Parse(userIdString);

            var ingresso = await _context.Ingressos.FindAsync(id);
            if (ingresso == null)
                return NotFound();

            // só permite deletar se for dono do ingresso
            if (ingresso.ClienteId != clienteId)
                return Forbid();

            _context.Ingressos.Remove(ingresso);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // buscar ingressos de um cliente específico
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
