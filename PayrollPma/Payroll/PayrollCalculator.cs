using PayrollPma.Models;

namespace PayrollPma.Payroll;

public sealed class PayrollCalculator(IPayrollRates rates) : IPayrollCalculator
{
    public PayrollResult Calculate(Employee employee)
    {
        // 1. Salario bruto mensual = rata por hora x horas mensuales
        var gross = Math.Round(employee.RatePerHour * employee.MonthlyHours, 6, MidpointRounding.AwayFromZero);

        // 2. IMPUESTO1: Seguro Social = bruto x 9.75%
        var socialSecurity = Math.Round(gross * rates.SocialSecurityRate, 6, MidpointRounding.AwayFromZero);

        // 3. IMPUESTO2: Seguro Educativo = bruto x 1.25%
        var educationalInsurance = Math.Round(gross * rates.EducationalInsuranceRate, 6, MidpointRounding.AwayFromZero);

        // 4. IMPUESTO3: ISR = ((bruto x 13) - 11000) x 15% / 13, floor en 0 si negativo
        var annualGross = gross * rates.IncomeTaxAnnualizationFactor;
        var taxableAnnual = annualGross - rates.IncomeTaxThreshold;
        var annualTax = taxableAnnual > 0 ? taxableAnnual * rates.IncomeTaxRate : 0m;
        var incomeTax = Math.Round(annualTax / rates.IncomeTaxAnnualizationFactor, 6, MidpointRounding.AwayFromZero);

        // 5. Neto = bruto - todas las deducciones
        var net = Math.Round(gross - socialSecurity - educationalInsurance - incomeTax, 6, MidpointRounding.AwayFromZero);

        return new PayrollResult(gross, socialSecurity, educationalInsurance, incomeTax, net);
    }
}
