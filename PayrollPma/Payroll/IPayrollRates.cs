namespace PayrollPma.Payroll;

public interface IPayrollRates
{
    decimal SocialSecurityRate { get; }            // 0.0975 (9.75%)
    decimal EducationalInsuranceRate { get; }      // 0.0125 (1.25%)
    decimal IncomeTaxAnnualizationFactor { get; } // 13
    decimal IncomeTaxThreshold { get; }           // 11000 (umbral anual)
    decimal IncomeTaxRate { get; }                // 0.15
}
