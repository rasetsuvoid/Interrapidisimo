namespace Interrapidisimo.Application.Common.Interfaces;

public interface IRsaEncryptionService
{
    string GetPublicKeyBase64();
    string Decrypt(string encryptedBase64);
}
