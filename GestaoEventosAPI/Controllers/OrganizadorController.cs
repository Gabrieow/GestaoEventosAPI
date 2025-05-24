using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Organizador")]  // Aqui exige que o usuário seja Organizador para TODOS os métodos
    public class OrganizadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizadorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]  // Permite acesso público para listar organizadores
        public async Task<IActionResult> GetAll()
        {
            var organizadores = await _context.Usuarios
                .Where(u => u.Role == Roles.Organizador)
                .ToListAsync();
            return Ok(organizadores);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]  // Permite acesso público para obter organizador por id
        public async Task<IActionResult> GetById(Guid id)
        {
            var organizador = await _context.Usuarios.FindAsync(id);

            if (organizador == null || organizador.Role != Roles.Organizador)
            {
                return NotFound();
            }

            return Ok(organizador);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Usuario organizador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            organizador.Role = Roles.Organizador;
            _context.Usuarios.Add(organizador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = organizador.Id }, organizador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Usuario organizador)
        {
            if (id != organizador.Id)
            {
                return BadRequest();
            }

            var existingOrganizador = await _context.Usuarios.FindAsync(id);

            if (existingOrganizador == null || existingOrganizador.Role != Roles.Organizador)
            {
                return NotFound();
            }

            existingOrganizador.Nome = organizador.Nome;
            existingOrganizador.Email = organizador.Email;

            _context.Entry(existingOrganizador).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var organizador = await _context.Usuarios.FindAsync(id);

            if (organizador == null || organizador.Role != Roles.Organizador)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(organizador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
