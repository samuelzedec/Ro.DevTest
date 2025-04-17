using System.Security.Cryptography.X509Certificates;

namespace RO.DevTest.Application.Settings;

public class JwtSettings
{
    public required string KeyPath { get; set; }
    public required string KeyPassword { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }

    /// <summary>
    /// Carrega um certificado PKCS#12 (.pfx/.p12) do sistema de arquivos,
    /// usa a senha para descriptografar seu conteúdo, e retorna um objeto X509Certificate2
    /// que contém o certificado e a chave privada prontos para uso na aplicação
    /// para assinar os tokens JWT.
    /// </summary>
    /// <remarks>
    /// Flags utilizadas:
    /// - X509KeyStorageFlags.Exportable: Permite que a chave privada associada ao certificado possa ser exportada posteriormente
    /// - X509KeyStorageFlags.PersistKeySet: Garante que a chave privada seja persistida no armazenamento de chaves do sistema
    /// </remarks>
    /// <returns>Certificado no formato válido para assinatura de tokens JWT</returns>
    public X509Certificate2 GenerateCertificate()
        => new X509Certificate2(
            KeyPath, KeyPassword,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
}