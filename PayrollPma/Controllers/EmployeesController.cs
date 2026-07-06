using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollPma.Data;
using PayrollPma.Models;
using PayrollPma.Payroll;
using PayrollPma.Services;

namespace PayrollPma.Controllers;

public class EmployeesController : Controller
{
    private readonly PayrollDbContext _db;
    private readonly PdfReportService _pdf;
    private readonly IPayrollCalculator _calculator;

    public EmployeesController(PayrollDbContext db, PdfReportService pdf, IPayrollCalculator calculator)
    {
        _db = db;
        _pdf = pdf;
        _calculator = calculator;
    }

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

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        var result = _calculator.Calculate(employee);
        ViewBag.Payroll = result;
        return View(employee);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id) return NotFound();
        if (!ModelState.IsValid) return View(employee);
        
        _db.Update(employee);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee != null)
        {
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult ExportPdf()
    {
        var employees = _db.Employees.ToList();
        var pdfBytes = _pdf.Generate(employees);
        return File(pdfBytes, "application/pdf", $"planilla_{DateTime.Now:yyyyMMdd}.pdf");
    }
}
