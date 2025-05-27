namespace GestaoEventosAPI.Application.DTOs
{
    public class AtracaoCreateDTO
    {
        public required string Nome { get; set; }
        public required string Categoria { get; set; }
        public required string Exigencias { get; set; }
        public required decimal Cache { get; set; }
        public required Guid EventoID { get; set; }
    }
}

// usado pra criar a atração em POST: (localhost)/api/atracao