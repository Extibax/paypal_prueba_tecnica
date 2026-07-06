using PayrollPma.Models;

namespace PayrollPma.Payroll;

public sealed class PayrollCalculator(IPayrollRates rates) : IPayrollCalculator
{
    public PayrollResult Calculate(Employee employee)
    {
        // 1. Salario bruto mensual = rata por hora × horas trabajadas
        var gross = employee.RatePerHour * employee.MonthlyHours;
        
        // 2. IMPUESTO1: Seguro Social (CSS empleado) — porcentaje plano del bruto
        var socialSecurity = Math.Round(gross * rates.SocialSecurityRate, 2, MidpointRounding.AwayFromZero);
        
        // 3. IMPUESTO2: Seguro Educativo — porcentaje plano del bruto
        var educationalInsurance = Math.Round(gross * rates.EducationalInsuranceRate, 2, MidpointRounding.AwayFromZero);
        
        // 4. IMPUESTO3: Impuesto sobre la Renta (ISR) — progresivo por tramos anuales
        var incomeTax = CalculateIncomeTax(gross, rates);
        
        // 5. Salario neto = bruto - todas las deducciones
        var net = Math.Round(gross - socialSecurity - educationalInsurance - incomeTax, 2, MidpointRounding.AwayFromZero);

        return new PayrollResult(gross, socialSecurity, educationalInsurance, incomeTax, net);
    }

    /// <summary>
    /// Calcula el ISR mensual. Si es progresivo, anualiza el salario, calcula por tramos,
    /// y divide entre 12. Si es flat, aplica la tasa única directamente.
    /// </summary>
    private static decimal CalculateIncomeTax(decimal monthlyGross, IPayrollRates rates)
    {
        // Si el impuesto NO es progresivo, aplicar tasa plana (flat)
        if (!rates.IncomeTaxIsProgressive)
        {
            var flatRate = rates.IncomeTaxBrackets.FirstOrDefault()?.Rate ?? 0m;
            return Math.Round(monthlyGross * flatRate, 2, MidpointRounding.AwayFromZero);
        }

        // ISR progresivo: anualizar salario, calcular impuesto anual, dividir entre 12
        var annualGross = monthlyGross * 12;
        var annualTax = 0m;

        // Recorrer cada tramo del impuesto y calcular la porción gravable
        foreach (var bracket in rates.IncomeTaxBrackets)
        {
            // Si el salario anual ya está por debajo del límite inferior, no hay más tramos que procesar
            if (annualGross <= bracket.LowerLimit) break;
            
            // Calcular cuánto del salario cae dentro de este tramo
            // Ejemplo: si salario=19200 y tramo es 11000-50000, entonces gravable = min(19200,50000)-11000 = 8200
            var taxableInBracket = Math.Min(annualGross, bracket.UpperLimit) - bracket.LowerLimit;
            
            // Si hay monto gravable en este tramo, multiplicar por la tasa del tramo
            if (taxableInBracket > 0)
                annualTax += taxableInBracket * bracket.Rate;
        }

        // Dividir impuesto anual entre 12 para obtener el monto mensual
        return Math.Round(annualTax / 12, 2, MidpointRounding.AwayFromZero);
    }
}
