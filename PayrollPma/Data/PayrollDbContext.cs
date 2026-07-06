using Microsoft.EntityFrameworkCore;
using PayrollPma.Models;

namespace PayrollPma.Data;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options) { }
    public DbSet<Employee> Employees => Set<Employee>();
}
