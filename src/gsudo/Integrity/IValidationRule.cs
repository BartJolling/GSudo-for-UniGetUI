using System.Diagnostics.CodeAnalysis;

namespace gsudo.Integrity;

public interface IValidationRule
{
    ValidationResult Validate();
}

public abstract record ValidationResult(string RuleName);

// Validation passed - continue checking
public record ValidationOK(string RuleName) : ValidationResult(RuleName);

// Validation passed - stop checking
[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used implicitly in record properties")]
public record ValidationSkip(string RuleName, string Code, string Message) : ValidationResult(RuleName);

// Validation failed - continue checking
[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used implicitly in record properties")]
public record ValidationFail(string RuleName, string Code, string Message) : ValidationResult(RuleName);

// Validation failed - stop checking
public record ValidationAbort(string RuleName, string Code, string Message) : ValidationFail(RuleName, Code, Message);
