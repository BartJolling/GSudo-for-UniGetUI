using System.Diagnostics;

namespace gsudo.Integrity;

public class CurrentProcessNameRule(Process currentProcess, string expectedName) : IValidationRule
{
    public ValidationResult Validate()
    {
        if (currentProcess.ProcessName != expectedName)
        {
            return new ValidationFail(nameof(CurrentProcessNameRule), "W_UNRECOGNIZED_ASSEMBLY_NAME",
                $"Expected process name \"{expectedName}\", but found \"{currentProcess.ProcessName}\".");
        }

        return new ValidationOK(nameof(CurrentProcessNameRule));
    }
}
