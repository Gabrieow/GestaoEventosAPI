using System.Security.Cryptography;
using System.Text;

namespace GestaoEventosAPI.Application
{
    public static class HashGenerator
    {
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}

// gera uma criptografia SHA256 da senha informada
// usado para gerar o hash da senha do usuário antes de salvar no banco de dados