namespace GestaoEventosAPI.Application.DTOs
{
    public class RegisterModel
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
    }
}