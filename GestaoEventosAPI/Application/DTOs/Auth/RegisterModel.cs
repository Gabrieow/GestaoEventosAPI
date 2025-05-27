namespace GestaoEventosAPI.Application.DTOs
{
    public class RegisterModel
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }
}

// modelo pra criar um usuário em POST: (localhost)/api/auth/register
// se bem sucedido, gera um "Cliente" com o email e senha informados