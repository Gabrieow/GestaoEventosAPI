namespace GestaoEventosAPI.Domain.Entities
{
    public class Atracao
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Categoria { get; set; }
        public required string Exigencias { get; set; }
        public required decimal Cache { get; set; }


        // vinculo com evento, todo evento PRECISA ter uma atração
        // e um evento pode ter varias atrações
        public Guid EventoID { get; set; }
        public Evento? Evento { get; set; }
    }
}