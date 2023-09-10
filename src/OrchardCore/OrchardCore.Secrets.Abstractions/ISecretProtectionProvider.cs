using System.Threading.Tasks;

namespace OrchardCore.Secrets;

public interface ISecretProtectionProvider
{
    Task<ISecretEncryptor> CreateEncryptorAsync(string encryptionSecret, string signingSecret);
    Task<ISecretDecryptor> CreateDecryptorAsync(string protectedData);
}