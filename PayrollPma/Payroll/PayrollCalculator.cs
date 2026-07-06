using PayrollPma.Models;

namespace PayrollPma.Payroll;

public sealed class PayrollCalculator(IPayrollRates rates) : IPayrollCalculator
{
    public PayrollResult Calculate(Employee employee)
    {
        var gross = employee.RatePerHour * employee.MonthlyHours;
        var socialSecurity = Math.Round(gross * rates.SocialSecurityRate, 2, MidpointRounding.AwayFromZero);
        var educationalInsurance = Math.Round(gross * rates.EducationalInsuranceRate, 2, MidpointRounding.AwayFromZero);
        var incomeTax = CalculateIncomeTax(gross, rates);
        var net = Math.Round(gross - socialSecurity - educationalInsurance - incomeTax, 2, MidpointRounding.AwayFromZero);

        return new PayrollResult(gross, socialSecurity, educationalInsurance, incomeTax, net);
    }

    private static decimal CalculateIncomeTax(decimal monthlyGross, IPayrollRates rates)
    {
        if (!rates.IncomeTaxIsProgressive)
        {
            var flatRate = rates.IncomeTaxBrackets.FirstOrDefault()?.Rate ?? 0m;
            return Math.Round(monthlyGross * flatRate, 2, MidpointRounding.AwayFromZero);
        }

        var annualGross = monthlyGross * 12;
        var annualTax = 0m;

        foreach (var bracket in rates.IncomeTaxBrackets)
        {
            if (annualGross <= bracket.LowerLimit) break;
            var taxableInBracket = Math.Min(annualGross, bracket.UpperLimit) - bracket.LowerLimit;
            if (taxableInBracket > 0)
                annualTax += taxableInBracket * bracket.Rate;
        }

        return Math.Round(annualTax / 12, 2, MidpointRounding.AwayFromZero);
    }
}
