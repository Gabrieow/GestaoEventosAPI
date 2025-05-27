namespace GestaoEventosAPI.Application.DTOs
{
    public class UpdateOrganizadorDto
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Cnpj { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Cep { get; set; }
    }
}

// usado para atualizar os dados do organizador em PUT: (localhost)/api/organizador/{id}