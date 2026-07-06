namespace PayrollPma.Payroll;

public interface IPayrollRates
{
    decimal SocialSecurityRate { get; }
    decimal EducationalInsuranceRate { get; }
    IReadOnlyList<TaxBracket> IncomeTaxBrackets { get; }
    bool IncomeTaxIsProgressive { get; }
}
