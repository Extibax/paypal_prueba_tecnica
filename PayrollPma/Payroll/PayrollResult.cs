namespace PayrollPma.Payroll;

public sealed record PayrollResult(
    decimal Gross,
    decimal SocialSecurity,
    decimal EducationalInsurance,
    decimal IncomeTax,
    decimal Net);
