using System;
using System.IO;
using System.Security.Cryptography;

namespace gsudo.Integrity;

public class HelperDllRule(string dllPath, string expectedHash) : IValidationRule
{
    private readonly string _dllPath = dllPath;
    private readonly string _expectedHash = expectedHash;

    public ValidationResult Validate()
    {
        if (!File.Exists(_dllPath))
        {
            return new ValidationFailed(nameof(HelperDllRule), "W_HELPER_DLL_NOT_FOUND",
                $"Could not find \"{_dllPath}\".");
        }

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(_dllPath);
        var fileHash = sha256.ComputeHash(stream);
        var hashString = Convert.ToHexString(fileHash).ToUpperInvariant();

        if (hashString != _expectedHash)
        {
            return new ValidationFailed(nameof(HelperDllRule), "W_HELPER_DLL_HASH_MISMATCH",
                $"Hash mismatch for \"{_dllPath}\". Expected \"{_expectedHash}\", got \"{hashString}\".");
        }

        return new ValidationOK(nameof(HelperDllRule));
    }
}
