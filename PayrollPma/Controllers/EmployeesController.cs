using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollPma.Data;
using PayrollPma.Models;

namespace PayrollPma.Controllers;

public class EmployeesController : Controller
{
    private readonly PayrollDbContext _db;
    public EmployeesController(PayrollDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Employees.ToListAsync());

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (!ModelState.IsValid) return View(employee);
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
