using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

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
            var atracoes = await _context.Atracoes.ToListAsync();
            return Ok(atracoes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous] // permite leitura sem autenticação
        public async Task<IActionResult> GetById(Guid id)
        {
            var atracao = await _context.Atracoes.FindAsync(id);
            if (atracao == null)
                return NotFound();

            return Ok(atracao);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem criar
        public async Task<IActionResult> Create([FromBody] Atracao atracao)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.Atracoes.AddAsync(atracao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = atracao.Id }, atracao);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem atualizar
        public async Task<IActionResult> Update(Guid id, [FromBody] Atracao atracao)
        {
            if (id != atracao.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.Atracoes.AnyAsync(a => a.Id == id);
            if (!exists)
                return NotFound();

            _context.Entry(atracao).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Organizador")] // só administradores e organizadores podem deletar
        public async Task<IActionResult> Delete(Guid id)
        {
            var atracao = await _context.Atracoes.FindAsync(id);
            if (atracao == null)
                return NotFound();

            _context.Atracoes.Remove(atracao);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
