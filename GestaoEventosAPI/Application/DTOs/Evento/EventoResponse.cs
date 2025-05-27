namespace GestaoEventosAPI.Application.DTOs
{
    public class EventoResponseDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public decimal PrecoIngresso { get; set; }
        public int QuantidadeIngressos { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public Guid OrganizadorId { get; set; }
        public OrganizadorDto Organizador { get; set; } = new OrganizadorDto();
    }
}

// retorna um evento completo, com todos os dados do organizador
// usado em GET: (localhost)/api/evento ou atracao/{id}