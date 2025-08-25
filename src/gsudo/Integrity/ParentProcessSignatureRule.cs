using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using gsudo.Helpers;
using Microsoft.Security.Extensions;

namespace gsudo.Integrity;

public class ParentProcessSignatureRule(Lazy<Process> parentProcess, string[] recognizedSubjects) : IValidationRule
{
    public ValidationResult Validate()
    {
        if (parentProcess?.Value is null)
        {
            return new ValidationFail(nameof(ParentProcessSignatureRule), "W_NULL_PARENT_PROCESS",
                "Parent process could not be determined.");
        }

        using var fs = File.OpenRead(parentProcess.Value.GetExeName());
        var sigInfo = FileSignatureInfo.GetFromFileStream(fs);

        if (sigInfo.State != SignatureState.SignedAndTrusted)
        {
            return new ValidationFail(nameof(ParentProcessSignatureRule), "W_UNTRUSTED_SIGNATURE_STATE",
                $"Signature state is \"{sigInfo.State}\", expected \"SignedAndTrusted\".");
        }

        if (!recognizedSubjects.Contains(sigInfo.SigningCertificate.Subject))
        {
            return new ValidationFail(nameof(ParentProcessSignatureRule), "W_UNKNOWN_SIGNATURE_SUBJECT",
                $"Subject \"{sigInfo.SigningCertificate.Subject}\" is not recognized.");
        }

        return new ValidationOK(nameof(ParentProcessSignatureRule));
    }
}
