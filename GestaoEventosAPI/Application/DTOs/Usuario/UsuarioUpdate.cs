using GestaoEventosAPI.Domain.Enums;

namespace GestaoEventosAPI.Application.DTOs
{
    public class UsuarioUpdateModel
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; } // opcional
        public Roles? Role { get; set; }
    }
}

// atualiza um usuario existente em PUT: (localhost)/api/usuario/{id}
// Senha é opcional, se não for informada, o valor atual será mantido