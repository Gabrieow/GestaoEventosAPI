namespace GestaoEventosAPI.Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Telefone { get; set; }
        public required string CPF { get; set; }
        public required string Endereco { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string CEP { get; set; }

        // vinculo com ingresso, todo cliente pode ter varios ingressos para varios eventos diferentes
        public ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }
}