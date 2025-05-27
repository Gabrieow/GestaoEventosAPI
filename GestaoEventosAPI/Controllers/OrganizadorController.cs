using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using GestaoEventosAPI.Application.DTOs;

namespace GestaoEventosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin, Organizador")]  // tem q ser admin ou org pra todos os metodos
    public class OrganizadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrganizadorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var organizadores = await _context.Organizadores
                .Include(o => o.Usuario)  // traz os dados do usuário relacionado
                .ToListAsync();

            return Ok(organizadores);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]  // permite acesso público
        public async Task<IActionResult> GetById(Guid id)
        {
            var organizador = await _context.Organizadores
                .Include(o => o.Usuario)  // traz os dados do usuário relacionado
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organizador == null)
            {
                return NotFound();
            }

            return Ok(organizador);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrganizadorCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = dto.SenhaHash,
                Role = Roles.Organizador
            };

            var organizador = new Organizador
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                CNPJ = dto.Cnpj,
                Endereco = dto.Endereco,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CEP = dto.Cep,
                Usuario = usuario // associar o usuário criado
            };

            _context.Usuarios.Add(usuario);
            _context.Organizadores.Add(organizador);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganizador(Guid id, [FromBody] UpdateOrganizadorDto dto)
        {
            var organizador = await _context.Organizadores
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (organizador == null)
                return NotFound();

            organizador.Nome = dto.Nome ?? string.Empty;
            organizador.Email = dto.Email ?? string.Empty;
            organizador.Telefone = dto.Telefone ?? string.Empty;
            organizador.CNPJ = dto.Cnpj ?? string.Empty;
            organizador.Endereco = dto.Endereco?? string.Empty; 
            organizador.Cidade = dto.Cidade ?? string.Empty;
            organizador.Estado = dto.Estado ?? string.Empty;
            organizador.CEP = dto.Cep ?? string.Empty;

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
