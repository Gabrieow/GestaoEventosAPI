using GestaoEventosAPI.Domain.Enums;

namespace GestaoEventosAPI.Application.DTOs
{
    public class UsuarioCreateModel
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }  // senha em texto puro
        public required Roles Role { get; set; }
    }
}

// cria um usuario em POST: (localhost)/api/usuario