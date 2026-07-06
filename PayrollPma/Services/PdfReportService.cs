using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PayrollPma.Models;
using PayrollPma.Payroll;
using System.Globalization;

namespace PayrollPma.Services;

public class PdfReportService(IPayrollCalculator calculator)
{
    public byte[] Generate(IEnumerable<Employee> employees)
    {
        var rows = employees
            .Select(e => (Employee: e, Result: calculator.Calculate(e)))
            .ToList();

        return Document.Create(container => container
            .Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4.Landscape());
                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeTable(c, rows));
                page.Footer().Element(ComposeFooter);
            }))
            .GeneratePdf();
    }

    static void ComposeHeader(IContainer c) => c.Column(col =>
    {
        col.Item().Text("Reporte de Planilla").FontSize(18).SemiBold();
        col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
            .FontSize(9).FontColor(Colors.Grey.Medium);
    });

    static void ComposeFooter(IContainer c) => c.AlignCenter()
        .Text(t => t.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium));

    static void ComposeTable(IContainer c, List<(Employee Employee, PayrollResult Result)> rows) =>
        c.Table(t =>
        {
            t.ColumnsDefinition(cd =>
            {
                cd.RelativeColumn(2);
                cd.RelativeColumn(2);
                cd.RelativeColumn(1.5f);
                cd.RelativeColumn(1.5f);
                cd.RelativeColumn(1.5f);
                cd.RelativeColumn(1.5f);
                cd.RelativeColumn(1.5f);
            });

            t.Header(h =>
            {
                h.Cell().Element(HeaderCell).Text("Nombre");
                h.Cell().Element(HeaderCell).Text("Apellido");
                h.Cell().Element(HeaderCell).AlignRight().Text("Bruto");
                h.Cell().Element(HeaderCell).AlignRight().Text("Seg. Social");
                h.Cell().Element(HeaderCell).AlignRight().Text("Seg. Educativo");
                h.Cell().Element(HeaderCell).AlignRight().Text("ISR");
                h.Cell().Element(HeaderCell).AlignRight().Text("Neto");
            });

            foreach (var (e, r) in rows)
            {
                t.Cell().Element(Cell).Text(e.Nombre);
                t.Cell().Element(Cell).Text(e.Apellido);
                t.Cell().Element(Cell).AlignRight().Text(r.Gross.ToString("N2", CultureInfo.InvariantCulture));
                t.Cell().Element(Cell).AlignRight().Text(r.SocialSecurity.ToString("N2", CultureInfo.InvariantCulture));
                t.Cell().Element(Cell).AlignRight().Text(r.EducationalInsurance.ToString("N2", CultureInfo.InvariantCulture));
                t.Cell().Element(Cell).AlignRight().Text(r.IncomeTax.ToString("N2", CultureInfo.InvariantCulture));
                t.Cell().Element(Cell).AlignRight().Text(r.Net.ToString("N2", CultureInfo.InvariantCulture));
            }
        });

    static IContainer HeaderCell(IContainer c) => c
        .Background(Colors.Grey.Lighten2)
        .Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Medium);

    static IContainer Cell(IContainer c) => c
        .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
        .Padding(5);
}
