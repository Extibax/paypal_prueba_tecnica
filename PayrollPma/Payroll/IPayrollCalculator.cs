using PayrollPma.Models;

namespace PayrollPma.Payroll;

public interface IPayrollCalculator
{
    PayrollResult Calculate(Employee employee);
}
