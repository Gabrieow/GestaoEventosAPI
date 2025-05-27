using GestaoEventosAPI.Domain.Enums;

namespace GestaoEventosAPI.Application.DTOs
{
    public class IngressoUpdateDTO
    {
        public Guid EventoId { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public TipoIngresso TipoIngresso { get; set; }
    }
}

// atualiza um ingresso existente em put /api/ingresso/{id}