namespace GestaoEventosAPI.Domain.Entities
{
    public class Organizador
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        public required string CNPJ { get; set; }
        public required string Endereco { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string CEP { get; set; }
    }
}