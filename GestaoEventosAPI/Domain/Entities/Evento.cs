namespace GestaoEventosAPI.Domain.Entities
{
    public class Evento
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public required DateTime DataInicio { get; set; }
        public required DateTime DataFim { get; set; }
        public required string Localizacao { get; set; }
        public required decimal PrecoIngresso { get; set; }
        public required int QuantidadeIngressos { get; set; }
        public required string Categoria { get; set; }

        // vinculo com organizador, todo evento PRECISA ter um organizador
        // e um organizador pode ter varios eventos
        public Guid OrganizadorId { get; set; }
        public Organizador? Organizador { get; set; }

        // vinculo com ingresso, todo evento pode ter varios ingressos
        // e um ingresso pode estar vinculado a um evento
        public ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }
}