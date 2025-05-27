using GestaoEventosAPI.Domain.Enums;

namespace GestaoEventosAPI.Application.DTOs
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Roles Role { get; set; }
    }
}

// usado para mostrar os dados do usuário em outros controllers sem vazar a senha (questao de segurança ne)