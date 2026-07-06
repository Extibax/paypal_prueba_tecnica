namespace PayrollPma.Payroll;

// ════════════════════════════════════════════════════════════════════
// PLACEHOLDER: Reemplazar estos valores con las fórmulas reales del país
// seleccionado cuando el director las proporcione.
// 
// IMPUESTO1 = SocialSecurityRate (porcentaje plano del salario bruto)
// IMPUESTO2 = EducationalInsuranceRate (porcentaje plano del salario bruto)
// IMPUESTO3 = IncomeTaxBrackets (tramos progresivos o flat, ver IncomeTaxIsProgressive)
// ════════════════════════════════════════════════════════════════════
public sealed class MockPayrollRates : IPayrollRates
{
    // TODO: Reemplazar con tasa real de IMPUESTO1 (seguro social)
    public decimal SocialSecurityRate => 0.09m; // 9% mock

    // TODO: Reemplazar con tasa real de IMPUESTO2 (seguro educativo)
    public decimal EducationalInsuranceRate => 0.0125m; // 1.25% mock

    // TODO: Reemplazar con tramos reales de IMPUESTO3 (ISR/renta)
    // Si es flat, usar un solo tramo con LowerLimit=0, UpperLimit=MaxValue
    public IReadOnlyList<TaxBracket> IncomeTaxBrackets => new[]
    {
        new TaxBracket(0m, 1000m, 0m),         // 0% hasta 1000
        new TaxBracket(1000m, 5000m, 0.15m),   // 15% de 1000 a 5000
        new TaxBracket(5000m, decimal.MaxValue, 0.25m) // 25% sobre 5000
    };

    // TODO: Cambiar a false si IMPUESTO3 es flat (porcentaje único)
    public bool IncomeTaxIsProgressive => true;
}
