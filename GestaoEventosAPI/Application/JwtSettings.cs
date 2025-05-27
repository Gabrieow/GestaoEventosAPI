namespace GestaoEventosAPI.Application
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
    }
}

// Configurações do JWT (authenticação via token)