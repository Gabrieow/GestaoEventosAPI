using System.Text.Json.Serialization;
using GestaoEventosAPI.Domain.Enums;

namespace GestaoEventosAPI.Domain.Entities
{
    public class Ingresso
    {
        public Guid Id { get; set; }

        public int Quantidade { get; set; }
        public required decimal Preco { get; set; }
        public required TipoIngresso TipoIngresso { get; set; } // Ex: VIP, PISTA, CAMAROTE, etc. (consultar TipoIngresso enum)
        public DateTime DataCompra { get; set; }

        // cliente que comprou
        public Guid ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // evento para o qual o ingresso foi comprado
        public Guid EventoId { get; set; }

        [JsonIgnore]
        public Evento? Evento { get; set; }
    }
}