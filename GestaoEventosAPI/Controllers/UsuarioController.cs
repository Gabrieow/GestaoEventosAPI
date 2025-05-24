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
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]  // Só Admin pode criar usuário
        public async Task<IActionResult> Create([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Só Admin pode atualizar usuário
        public async Task<IActionResult> Update(Guid id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest();

            var existingUser = await _context.Usuarios.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.Nome = usuario.Nome;
            existingUser.Email = usuario.Email;
            existingUser.Role = usuario.Role;

            _context.Entry(existingUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Só Admin pode deletar usuário
        public async Task<IActionResult> Delete(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
