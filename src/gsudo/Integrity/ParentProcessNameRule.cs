using System;
using System.Diagnostics;
using System.Linq;

namespace gsudo.Integrity;

public class ParentProcessNameRule(Lazy<Process> parentProcess, string[] recognizedNames) : IValidationRule
{
    public ValidationResult Validate()
    {
        if (parentProcess?.Value is null)
        {
            return new ValidationFail(nameof(ParentProcessNameRule), "W_NULL_PARENT_PROCESS",
                "Parent process could not be determined.");
        }

        if (!recognizedNames.Contains(parentProcess.Value.ProcessName))
        {
            return new ValidationFail(nameof(ParentProcessNameRule), "W_UNRECOGNIZED_PARENT_ASSEMBLY_NAME",
                $"Parent process name \"{parentProcess.Value.ProcessName}\" is not recognized.");
        }

        return new ValidationOK(nameof(ParentProcessNameRule));
    }
}
