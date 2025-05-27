using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GestaoEventosAPI.Application.DTOs;
using System.Security.Claims;
using GestaoEventosAPI.Domain.Enums;

#nullable disable

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // exige autenticação para todos os métodos
    public class AtracaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AtracaoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous] // permite leitura sem autenticação
        public async Task<IActionResult> GetAll()
        {
            // Inclui o Evento na consulta para evitar evento null
            var atracoes = await _context.Atracoes
                .Include(a => a.Evento)
                    .ThenInclude(e => e.Organizador)
                .ToListAsync();

            return Ok(atracoes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // permite leitura sem autenticação
        public async Task<IActionResult> GetById(Guid id)
        {
            // Inclui o Evento na consulta para evitar evento null
            var atracao = await _context.Atracoes
                .Include(a => a.Evento)
                    .ThenInclude(e => e.Organizador)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (atracao == null)
                return NotFound();

            return Ok(atracao);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem criar
        public async Task<IActionResult> Create([FromBody] AtracaoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var evento = await _context.Eventos.FindAsync(dto.EventoID);
            if (evento == null)
                return NotFound("Evento não encontrado.");

            var atracao = new Atracao
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Categoria = dto.Categoria,
                Exigencias = dto.Exigencias,
                Cache = dto.Cache,
                EventoID = dto.EventoID,
                Evento = evento
            };

            await _context.Atracoes.AddAsync(atracao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = atracao.Id }, atracao);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem atualizar
        public async Task<IActionResult> Update(Guid id, [FromBody] AtracaoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var atracao = await _context.Atracoes
                .Include(a => a.Evento)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (atracao == null)
                return NotFound();

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
                return Unauthorized();

            if (usuario.Role == Roles.Organizador)
            {
                var organizador = await _context.Organizadores
                    .FirstOrDefaultAsync(o => o.UsuarioId == usuario.Id);

                if (atracao.Evento == null || atracao.Evento.OrganizadorId != organizador?.Id)
                    return Forbid();
            }

            atracao.Nome = dto.Nome;
            atracao.Categoria = dto.Categoria;
            atracao.Exigencias = dto.Exigencias;
            atracao.Cache = dto.Cache;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem deletar
        public async Task<IActionResult> Delete(Guid id)
        {
            var atracao = await _context.Atracoes
                .Include(a => a.Evento)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (atracao == null)
                return NotFound();

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario != null && usuario.Role == Roles.Organizador)
            {
                var organizador = await _context.Organizadores
                    .FirstOrDefaultAsync(o => o.UsuarioId == usuario.Id);

                if (atracao.Evento == null || organizador == null || atracao.Evento.OrganizadorId != organizador.Id)
                    return Forbid();
            }

            _context.Atracoes.Remove(atracao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
