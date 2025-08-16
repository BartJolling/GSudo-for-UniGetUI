using System.Diagnostics.CodeAnalysis;

namespace gsudo.Integrity;

public interface IValidationRule
{
    ValidationResult Validate();
}

public abstract record ValidationResult(string RuleName);

public record ValidationOK(string RuleName) : ValidationResult(RuleName);

[SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Used implicitly in record properties")]
public record ValidationFailed(string RuleName, string Code, string Message) : ValidationResult(RuleName);