namespace GestaoEventosAPI.Application.DTOs
{
    using GestaoEventosAPI.Domain.Enums;
    public class OrganizadorCreateDto
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string SenhaHash { get; set; }
        public Roles Role { get; set; } = Roles.Organizador;

        // Dados específicos do organizador
        public required string Telefone { get; set; }
        public required string Cnpj { get; set; }
        public required string Endereco { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Cep { get; set; }
    }
}

// cria um organizador em post /api/organizador