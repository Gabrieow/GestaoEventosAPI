namespace GestaoEventosAPI.Application.DTOs
{
    public class EventoReadDto
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

        public required OrganizadorDto Organizador { get; set; }
    }
}

// usado para ler um evento em GET: (localhost)/api/evento/{id}