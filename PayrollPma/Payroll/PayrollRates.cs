using Microsoft.Extensions.Configuration;

namespace PayrollPma.Payroll;

/// <summary>
/// Tasas de planilla leídas desde appsettings.json (sección "PayrollRates").
/// Valores alineados a la fórmula del paper: SS 9.75%, SE 1.25%, ISR flat 15%
/// sobre exceso de 11000 anualizado x13.
/// </summary>
public sealed class PayrollRates : IPayrollRates
{
    public decimal SocialSecurityRate { get; }
    public decimal EducationalInsuranceRate { get; }
    public decimal IncomeTaxAnnualizationFactor { get; }
    public decimal IncomeTaxThreshold { get; }
    public decimal IncomeTaxRate { get; }

    public PayrollRates(IConfiguration configuration)
    {
        var section = configuration.GetSection("PayrollRates");
        SocialSecurityRate = section.GetValue<decimal>(nameof(SocialSecurityRate));
        EducationalInsuranceRate = section.GetValue<decimal>(nameof(EducationalInsuranceRate));
        IncomeTaxAnnualizationFactor = section.GetValue<decimal>(nameof(IncomeTaxAnnualizationFactor));
        IncomeTaxThreshold = section.GetValue<decimal>(nameof(IncomeTaxThreshold));
        IncomeTaxRate = section.GetValue<decimal>(nameof(IncomeTaxRate));
    }
}
