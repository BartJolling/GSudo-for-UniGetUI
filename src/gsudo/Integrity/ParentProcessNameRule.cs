using System;
using System.Diagnostics;
using System.Linq;

namespace gsudo.Integrity;

public class ParentProcessNameRule(Process parentProcess, string[] recognizedNames) : IValidationRule
{
    private readonly Process _parentProcess = parentProcess;
    private readonly string[] _recognizedNames = recognizedNames;

    public ValidationResult Validate()
    {
        if (_parentProcess == null)
        {
            return new ValidationFailed(nameof(ParentProcessNameRule), "W_NULL_PARENT_PROCESS",
                "Parent process could not be determined.");
        }

        if (!_recognizedNames.Contains(_parentProcess.ProcessName))
        {
            return new ValidationFailed(nameof(ParentProcessNameRule), "W_UNRECOGNIZED_PARENT_ASSEMBLY_NAME",
                $"Parent process name \"{_parentProcess.ProcessName}\" is not recognized.");
        }

        return new ValidationOK(nameof(ParentProcessNameRule));
    }
}
