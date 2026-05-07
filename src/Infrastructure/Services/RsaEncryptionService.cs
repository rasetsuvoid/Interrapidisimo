using Interrapidisimo.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Interrapidisimo.Infrastructure.Services;

public sealed class RsaEncryptionService : IRsaEncryptionService, IDisposable
{
    private readonly RSA _rsa = RSA.Create(2048);

    public string GetPublicKeyBase64() =>
        Convert.ToBase64String(_rsa.ExportSubjectPublicKeyInfo());

    public string Decrypt(string encryptedBase64)
    {
        var encrypted = Convert.FromBase64String(encryptedBase64);
        var decrypted = _rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decrypted);
    }

    public void Dispose() => _rsa.Dispose();
}
