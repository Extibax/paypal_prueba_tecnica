using PayrollPma.Models;

namespace PayrollPma.Payroll;

public sealed class PayrollCalculator(IPayrollRates rates) : IPayrollCalculator
{
    public PayrollResult Calculate(Employee employee) => throw new NotImplementedException();
}
