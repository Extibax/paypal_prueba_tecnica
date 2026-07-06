using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollPma.Data;
using PayrollPma.Models;
using PayrollPma.Services;

namespace PayrollPma.Controllers;

public class EmployeesController : Controller
{
    private readonly PayrollDbContext _db;
    private readonly PdfReportService _pdf;

    public EmployeesController(PayrollDbContext db, PdfReportService pdf)
    {
        _db = db;
        _pdf = pdf;
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

    public IActionResult ExportPdf()
    {
        var employees = _db.Employees.ToList();
        var pdfBytes = _pdf.Generate(employees);
        return File(pdfBytes, "application/pdf", $"planilla_{DateTime.Now:yyyyMMdd}.pdf");
    }
}
