using System.ComponentModel.DataAnnotations;

namespace GestaoEventosAPI.Application.DTOs
{
    public class AtracaoUpdateDTO
    {
        [Required]
        public required string Nome { get; set; }

        [Required]
        public required string Categoria { get; set; }

        public required string Exigencias { get; set; }

        [Required]
        public required decimal Cache { get; set; }
    }
}

// usado pra atualizar a atração em PUT: (localhost)/api/atracao/{id}