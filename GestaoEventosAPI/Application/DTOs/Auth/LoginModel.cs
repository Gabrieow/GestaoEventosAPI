namespace GestaoEventosAPI.Application.DTOs
{
    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }
}

// usado pra logar o usuario registrado em POST: (localhost)/api/auth/login
// retorna o token de autenticação se o login for bem-sucedido