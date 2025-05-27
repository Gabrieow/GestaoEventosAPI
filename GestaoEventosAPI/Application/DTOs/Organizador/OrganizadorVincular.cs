namespace GestaoEventosAPI.Application.DTOs
{
    public class OrganizadorVincularDto
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        public required string Cnpj { get; set; }
        public required string Endereco { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Cep { get; set; }
    }
}

// usado para tornar um cliente um organizador em POST: (localhost)/api/organizador/vincular-existente?usuarioId={id do usuario}