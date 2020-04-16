using JetBrains.Annotations;

namespace Tolltech.Muser.Settings
{
    public interface ICryptoService
    {
        [NotNull]
        string Encrypt([NotNull] string src, [NotNull] string cryptoKey);

        [CanBeNull]
        string Decrypt([NotNull] string src, [NotNull] string cryptoKey);
    }
}