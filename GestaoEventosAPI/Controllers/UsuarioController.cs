using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GestaoEventosAPI.Application.DTOs;
using GestaoEventosAPI.Application;
using GestaoEventosAPI.Domain.Enums;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Usuarios.Any(u => u.Email == model.Email))
                return Conflict("Já existe um usuário com este e-mail.");

            var usuario = new Usuario
            {
                Nome = model.Nome,
                Email = model.Email,
                SenhaHash = HashGenerator.ComputeSha256Hash(model.Senha),
                Role = model.Role
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, UsuarioUpdateModel model)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (model.Nome != null) usuario.Nome = model.Nome;
            if (model.Email != null) usuario.Email = model.Email;
            if (model.Senha != null) usuario.SenhaHash = HashGenerator.ComputeSha256Hash(model.Senha);
            if (model.Role.HasValue) usuario.Role = model.Role.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // só adm pode deletar usuário
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
