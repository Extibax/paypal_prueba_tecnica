namespace PayrollPma.Payroll;

/// <summary>
/// Tasas de planilla de Panama (valores referenciales).
/// CSS empleado: 9.75%, CSS patrono: 12.25% (solo el empleado cuenta para deducción).
/// Seguro Educativo: 1.25% empleado.
/// ISR: progresivo por tramos anuales.
/// </summary>
public sealed class PayrollRates : IPayrollRates
{
    public decimal SocialSecurityRate => 0.0975m;
    public decimal EducationalInsuranceRate => 0.0125m;

    public IReadOnlyList<TaxBracket> IncomeTaxBrackets =>
    [
        new(0, 11_000m, 0m),
        new(11_000m, 50_000m, 0.15m),
        new(50_000m, int.MaxValue, 0.25m),
    ];

    public bool IncomeTaxIsProgressive => true;
}
