using Microsoft.EntityFrameworkCore;
using PayrollPma.Payroll;
using PayrollPma.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<PayrollPma.Data.PayrollDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IPayrollRates, PayrollRates>();
builder.Services.AddSingleton<IPayrollCalculator, PayrollCalculator>();
builder.Services.AddSingleton<PdfReportService>();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// Seed mock data para demostración
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayrollPma.Data.PayrollDbContext>();
    db.Database.EnsureCreated();
    if (!db.Employees.Any())
    {
        db.Employees.AddRange(
            new PayrollPma.Models.Employee { Nombre = "Juan", Apellido = "Bedoya", RatePerHour = 10m, MonthlyHours = 160 },
            new PayrollPma.Models.Employee { Nombre = "María", Apellido = "López", RatePerHour = 15m, MonthlyHours = 120 },
            new PayrollPma.Models.Employee { Nombre = "Carlos", Apellido = "Ruiz", RatePerHour = 25m, MonthlyHours = 80 }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
