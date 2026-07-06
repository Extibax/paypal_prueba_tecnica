namespace PayrollPma.Payroll;

public sealed record TaxBracket(decimal LowerLimit, decimal UpperLimit, decimal Rate);
