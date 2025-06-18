using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;
using GestaoEventosAPI.Data;
using GestaoEventosAPI.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using GestaoEventosAPI.Application;

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
            var dtos = eventos.Select(ToReadDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            return Ok(ToReadDto(evento));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Create(EventoCreateDto eventoDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null)
                return Unauthorized();

            Guid usuarioIdLogado = Guid.Parse(userIdClaim.Value);

            var organizador = await _context.Organizadores
                .FirstOrDefaultAsync(o => o.UsuarioId == usuarioIdLogado);

            if (organizador == null)
                return BadRequest("Usuário logado não está associado a nenhum organizador.");

            Evento novoEvento = new Evento
            {
                Nome = eventoDto.Nome,
                Descricao = eventoDto.Descricao,
                DataInicio = eventoDto.DataInicio,
                DataFim = eventoDto.DataFim,
                Localizacao = eventoDto.Localizacao,
                PrecoIngresso = eventoDto.PrecoIngresso,
                QuantidadeIngressos = eventoDto.QuantidadeIngressos,
                Categoria = eventoDto.Categoria,

                OrganizadorId = organizador.Id
            };

            _context.Eventos.Add(novoEvento);
            await _context.SaveChangesAsync();

            Evento? eventoComDados = await _context.Eventos
                .Include(e => e.Organizador)
                    .ThenInclude(o => o != null ? o.Usuario : null)
                .FirstOrDefaultAsync(e => e.Id == novoEvento.Id);

            if (eventoComDados == null)
                return NotFound();

            return CreatedAtAction(nameof(GetById), new { id = eventoComDados.Id }, ToReadDto(eventoComDados));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventoCreateDto eventoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var eventoExistente = await _context.Eventos.FindAsync(id);
            if (eventoExistente == null)
                return NotFound();

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var usuarioLogado = await _context.Usuarios.FindAsync(userId);
            if (usuarioLogado == null || (usuarioLogado.Role != Roles.Organizador && usuarioLogado.Role != Roles.Admin))
                return Forbid();

            if (usuarioLogado.Role == Roles.Organizador)
            {
                var organizadorLogado = await _context.Organizadores
                    .FirstOrDefaultAsync(o => o.UsuarioId == usuarioLogado.Id);

                if (organizadorLogado == null)
                    return Forbid();

                if (eventoExistente.OrganizadorId != organizadorLogado.Id)
                    return Forbid();
            }

            eventoExistente.Nome = eventoDto.Nome;
            eventoExistente.Descricao = eventoDto.Descricao;
            eventoExistente.DataInicio = eventoDto.DataInicio;
            eventoExistente.DataFim = eventoDto.DataFim;
            eventoExistente.Localizacao = eventoDto.Localizacao;
            eventoExistente.PrecoIngresso = eventoDto.PrecoIngresso;
            eventoExistente.QuantidadeIngressos = eventoDto.QuantidadeIngressos;
            eventoExistente.Categoria = eventoDto.Categoria;

            await _context.SaveChangesAsync();

            Evento? eventoAtualizado = await _context.Eventos
                .Include(e => e.Organizador)
                .ThenInclude(o => o != null ? o.Usuario : null)
                .FirstOrDefaultAsync(e => e.Id == eventoExistente.Id);

            if (eventoAtualizado == null)
                return NotFound();

            var dto = ToReadDto(eventoAtualizado);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound();

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var usuarioLogado = await _context.Usuarios.FindAsync(userId);
            if (usuarioLogado == null || (usuarioLogado.Role != Roles.Organizador && usuarioLogado.Role != Roles.Admin))
                return Forbid();

            if (usuarioLogado.Role == Roles.Organizador)
            {
                var organizadorLogado = await _context.Organizadores
                    .FirstOrDefaultAsync(o => o.UsuarioId == usuarioLogado.Id);

                if (organizadorLogado == null)
                    return Forbid();

                if (evento.OrganizadorId != organizadorLogado.Id)
                    return Forbid();
            }

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

            var dtos = eventos.Select(ToReadDto);
            return Ok(dtos);
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

            var resultado = await eventos.ToListAsync();
            var dtos = resultado.Select(ToReadDto);

            return Ok(dtos);
        }

        [HttpGet("organizador/{id}")]
        [Authorize(Roles = "Admin, Organizador")]
        public async Task<IActionResult> GetByOrganizador(Guid id)
        {
            var eventos = await _context.Eventos.Where(e => e.OrganizadorId == id).ToListAsync();

            if (eventos == null || !eventos.Any())
                return NotFound();

            var dtos = eventos.Select(ToReadDto);
            return Ok(dtos);
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
            var dtos = eventos.Select(ToReadDto);

            return Ok(dtos);
        }

        private EventoReadDto ToReadDto(Evento? e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e), "Evento não pode ser nulo.");
            }

            var organizador = _context.Organizadores
                .Include(o => o.Usuario)
                .FirstOrDefault(o => o.Id == e.OrganizadorId);

            if (organizador == null)
            {
                throw new Exception("Organizador não encontrado.");
            }

            return new EventoReadDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Descricao = e.Descricao,
                DataInicio = e.DataInicio,
                DataFim = e.DataFim,
                Localizacao = e.Localizacao,
                PrecoIngresso = e.PrecoIngresso,
                QuantidadeIngressos = e.QuantidadeIngressos,
                Categoria = e.Categoria,

                Organizador = new OrganizadorDto
                {
                    Id = organizador.Id,
                    Nome = organizador.Nome,
                    Email = organizador.Email,
                    Telefone = organizador.Telefone,
                    CNPJ = organizador.CNPJ,
                    Endereco = organizador.Endereco,
                    Cidade = organizador.Cidade,
                    Estado = organizador.Estado,
                    CEP = organizador.CEP,
                    Usuario = new UsuarioDto
                    {
                        Id = organizador.Usuario.Id,
                        Nome = organizador.Usuario.Nome,
                        Email = organizador.Usuario.Email,
                        Role = organizador.Usuario.Role
                    }
                }
            };
        }
    }
}
