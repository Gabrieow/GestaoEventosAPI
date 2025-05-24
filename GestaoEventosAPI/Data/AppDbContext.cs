using Microsoft.EntityFrameworkCore;
using GestaoEventosAPI.Domain.Entities;

namespace GestaoEventosAPI.Data
{
    public class AppDbContext : DbContext
    {
        // construtor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // entities
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Atracao> Atracoes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}
