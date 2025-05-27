namespace GestaoEventosAPI.Application.DTOs
{
    public class ClienteUpdate
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? CPF { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? CEP { get; set; }
    }
}

// usado para o cliente atualizar os dados em PUT: (localhost)/api/cliente/{id}
// Note: All properties are optional to allow partial updates.