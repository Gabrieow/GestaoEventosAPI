namespace GestaoEventosAPI.Domain.Enums
{
    public enum Roles
    {
        Admin,
        Organizador,
        Cliente
    }
}

// admin = pode criar, editar e excluir eventos, atracoes, clientes, organizadores e ingressos
// organizador = pode criar e editar eventos e atracoes e visualizar ingressos, mas nao pode excluir nada
// cliente = pode visualizar eventos, atracoes e ingressos, mas nao pode criar, editar ou excluir nada