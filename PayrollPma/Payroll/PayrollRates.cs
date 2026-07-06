using Microsoft.Extensions.Configuration;

namespace PayrollPma.Payroll;

/// <summary>
/// Tasas de planilla leídas desde appsettings.json (sección "PayrollRates").
/// Para cambiar las tasas, editar el archivo de configuración sin tocar código.
/// </summary>
public sealed class PayrollRates : IPayrollRates
{
    public decimal SocialSecurityRate { get; }
    public decimal EducationalInsuranceRate { get; }
    public IReadOnlyList<TaxBracket> IncomeTaxBrackets { get; }
    public bool IncomeTaxIsProgressive { get; }

    public PayrollRates(IConfiguration configuration)
    {
        var section = configuration.GetSection("PayrollRates");
        
        SocialSecurityRate = section.GetValue<decimal>(nameof(SocialSecurityRate));
        EducationalInsuranceRate = section.GetValue<decimal>(nameof(EducationalInsuranceRate));
        IncomeTaxIsProgressive = section.GetValue<bool>(nameof(IncomeTaxIsProgressive));
        
        var brackets = new List<TaxBracket>();
        var bracketsSection = section.GetSection("IncomeTaxBrackets");
        foreach (var bracket in bracketsSection.GetChildren())
        {
            var lower = bracket.GetValue<decimal>(nameof(TaxBracket.LowerLimit));
            var upper = bracket.GetValue<decimal>(nameof(TaxBracket.UpperLimit));
            var rate = bracket.GetValue<decimal>(nameof(TaxBracket.Rate));
            brackets.Add(new TaxBracket(lower, upper, rate));
        }
        IncomeTaxBrackets = brackets;
    }
}
