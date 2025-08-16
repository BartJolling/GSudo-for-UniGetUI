using System.Diagnostics;
using System.IO;
using System.Linq;
using gsudo.Helpers;
using Microsoft.Security.Extensions;

namespace gsudo.Integrity;

public class ParentProcessSignatureRule(Process parentProcess, string[] recognizedSubjects) : IValidationRule
{
    private readonly Process _parentProcess = parentProcess;
    private readonly string[] _recognizedSubjects = recognizedSubjects;

    public ValidationResult Validate()
    {
        if (_parentProcess == null)
        {
            return new ValidationFailed(nameof(ParentProcessSignatureRule), "W_NULL_PARENT_PROCESS",
                "Parent process could not be determined.");
        }

        using var fs = File.OpenRead(_parentProcess.GetExeName());
        var sigInfo = FileSignatureInfo.GetFromFileStream(fs);

        if (sigInfo.State != SignatureState.SignedAndTrusted)
        {
            return new ValidationFailed(nameof(ParentProcessSignatureRule), "W_UNTRUSTED_SIGNATURE_STATE",
                $"Signature state is \"{sigInfo.State}\", expected \"SignedAndTrusted\".");
        }

        if (!_recognizedSubjects.Contains(sigInfo.SigningCertificate.Subject))
        {
            return new ValidationFailed(nameof(ParentProcessSignatureRule), "W_UNKNOWN_SIGNATURE_SUBJECT",
                $"Subject \"{sigInfo.SigningCertificate.Subject}\" is not recognized.");
        }

        return new ValidationOK(nameof(ParentProcessSignatureRule));
    }
}
