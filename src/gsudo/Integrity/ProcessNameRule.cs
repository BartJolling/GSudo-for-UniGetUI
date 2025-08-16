using System.Diagnostics;

namespace gsudo.Integrity;

public class ProcessNameRule(Process currentProcess, string expectedName) : IValidationRule
{
    private readonly Process _currentProcess = currentProcess;
    private readonly string _expectedName = expectedName;

    public ValidationResult Validate()
    {
        if (_currentProcess.ProcessName != _expectedName)
        {
            return new ValidationFailed(nameof(ProcessNameRule), "W_UNRECOGNIZED_ASSEMBLY_NAME",
                $"Expected process name \"{_expectedName}\", but found \"{_currentProcess.ProcessName}\".");
        }

        return new ValidationOK(nameof(ProcessNameRule));
    }
}
