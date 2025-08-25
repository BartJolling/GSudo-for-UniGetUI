using System;
using System.Diagnostics;
using gsudo.Helpers;
using gsudo.Integrity;

internal class ParentProcessAlive(Process currentProcess, Lazy<Process> parentProcess) : IValidationRule
{
    public string RuleName => nameof(ParentProcessAlive);

    public ValidationResult Validate()
    {
        var directParentProcessId = ProcessHelper.GetParentProcessId(currentProcess);
        var parentName = ProcessHelper.GetExeNameFromSnapshot(directParentProcessId);

        // Is direct parent still alive
        if (!ProcessHelper.IsProcessAlive(directParentProcessId))
        {
            return new ValidationAbort(RuleName, "W_NULL_DIRECT_PARENT_PROCESS",
                $"Direct parent process '{parentName}' with PID {directParentProcessId} is not alive.");
        }

        // Attach to parent process
        Process directParentProcess;

        try
        {
            directParentProcess = Process.GetProcessById(directParentProcessId);

            if (directParentProcess is null)
            {
                return new ValidationAbort(RuleName, "W_NULL_DIRECT_PARENT_PROCESS",
                    $"Direct parent process '{parentName}' with PID {directParentProcessId} could not be found.");
            }
        }
        catch (Exception ex)
        {
            return new ValidationAbort(RuleName, "W_NULL_DIRECT_PARENT_PROCESS",
                $"Direct parent process '{parentName}' with PID {directParentProcessId} could not be accessed: {ex.Message}");
        }

        if (currentProcess.GetExeName() == directParentProcess.GetExeName())
        {
            // Current process was spawned by itself,
            // Send signal to skip rest of the checks
            return new ValidationSkip(RuleName, "I_SELF_SPAWN", "Self-elevation detected.");
        }

        try
        {
            // Check original parent, recursively
            if (parentProcess.Value is null)
            {
                return new ValidationAbort(RuleName, "W_NULL_ORIGINAL_PARENT_PROCESS",
                    $"Original parent process could not be determined.");
            }
        }
        catch (Exception ex)
        {
            return new ValidationAbort(RuleName, "W_NULL_ORIGINAL_PARENT_PROCESS",
                $"Original parent process could not be determined: {ex.Message}");
        }

        return new ValidationOK(RuleName);
    }
}