using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using gsudo.Helpers;

namespace gsudo.Integrity;

public static class IntegrityCheck
{
    private const string VALID_PROCESS_NAME = "UniGetUI Elevator";
    private const string VALID_HELPER_DLL_NAME = "getfilesiginforedist.dll";
    private const string VALID_HELPER_DLL_HASH = "153EEFB2EAFA8B2B909854CC1F941350EFB1170E179A299DE8836B8EC5CE6A7A";
    private static readonly string[] VALID_PARENT_PROCESS_NAMES =
    [
        "UniGetUI",
        "WingetUI",
#if DEBUG
        "vsdbg-ui",
#endif
    ];
    private static readonly string[] VALID_PARENT_SUBJECTS =
    [
        "CN=Marti Climent Lopez, O=Marti Climent Lopez, L=Barcelona, S=Barcelona, C=ES",
        "CN=\"Open Source Developer, Martí Climent López\", O=Open Source Developer, L=Barcelona, S=Barcelona, C=ES",
#if DEBUG
        "CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US",
#endif
    ];

    public static List<ValidationFail> VerifyCallerProcess()
    {
        var results = new List<ValidationFail>();

        var current = Process.GetCurrentProcess();
        var parentLazy = new Lazy<Process>(() => GetParentProcess());

        var rules = new List<IValidationRule>
        {
            new ParentProcessAlive(current, parentLazy), // Must be first to do basic parent process validation
            new CurrentProcessNameRule(current, VALID_PROCESS_NAME),
            new HelperDllRule(Path.Combine(Path.GetDirectoryName(current.MainModule?.FileName) ?? "", VALID_HELPER_DLL_NAME), VALID_HELPER_DLL_HASH),
            new ParentProcessNameRule(parentLazy, VALID_PARENT_PROCESS_NAMES),
            new ParentProcessSignatureRule(parentLazy, VALID_PARENT_SUBJECTS)
        };

        foreach (var rule in rules)
        {
            var result = rule.Validate();

            if (result is ValidationAbort aborted)
            {
                results.Add(aborted);
                Logger.Instance.Log($"[{aborted.RuleName}] {aborted.Code}: {aborted.Message} Skipping further checks", LogLevel.Error);
                return results;
            }
            else if (result is ValidationFail failed)
            {
                results.Add(failed);
                Logger.Instance.Log($"[{failed.RuleName}] {failed.Code}: {failed.Message}", LogLevel.Warning);
            }
            else if (result is ValidationSkip skipped)
            {
                Logger.Instance.Log($"[{skipped.RuleName}] {skipped.Code}: {skipped.Message} Skipping further checks", LogLevel.Info);
                return results;
            }
            else
            {
                Logger.Instance.Log($"[{result.RuleName}] SUCCESS.", LogLevel.Info);
            }
        }

        return results;
    }

    public static Process GetParentProcess()
    {
        var parentProcess = Process.GetCurrentProcess();

        while (parentProcess?.ProcessName == VALID_PROCESS_NAME)
            parentProcess = parentProcess.GetParentProcess();

        return parentProcess;
    }
}