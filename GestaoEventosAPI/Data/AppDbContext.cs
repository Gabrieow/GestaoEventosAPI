using Microsoft.EntityFrameworkCore;
using GestaoEventosAPI.Domain.Entities;
using GestaoEventosAPI.Domain.Enums;

public class AppDbContext : DbContext
{
    public DbSet<Atracao> Atracoes { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Ingresso> Ingressos { get; set; }
    public DbSet<Organizador> Organizadores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


    }
}
