using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PayrollPma.Models;
using PayrollPma.Payroll;
using System.Globalization;

namespace PayrollPma.Services;

public class PdfReportService(IPayrollCalculator calculator)
{
    static readonly Color BrandBlue = Color.FromHex("#1E3A5F");
    static readonly Color ZebraLight = Color.FromHex("#F4F6F8");
    static readonly Color BorderColor = Color.FromHex("#E2E8F0");
    static readonly Color GreenDark = Color.FromHex("#1B7340");
    static readonly Color TotalsBg = Color.FromHex("#EEF2F7");

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
                page.Header().Element(c => ComposeHeader(c, rows.Count));
                page.Content().Element(c => ComposeTable(c, rows));
                page.Footer().Element(ComposeFooter);
            }))
            .GeneratePdf();
    }

    static void ComposeHeader(IContainer c, int employeeCount)
    {
        c.Column(col =>
        {
            // Band of color full-width
            col.Item().Background(BrandBlue).Padding(12).Column(inner =>
            {
                inner.Item().Text("Reporte de Planilla")
                    .FontSize(20).FontColor(Colors.White).SemiBold();
                inner.Item().Text("Sosa y Cia., S.A.")
                    .FontSize(10).FontColor(Colors.Grey.Lighten3);
            });

            // Info line below band
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
                row.RelativeItem().AlignRight().Text($"Empleados: {employeeCount}")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    static void ComposeFooter(IContainer c)
    {
        c.Row(row =>
        {
            row.RelativeItem().AlignLeft().Text("Planilla RRHH - Confidencial")
                .FontSize(8).FontColor(Colors.Grey.Medium);
            row.RelativeItem().AlignRight().Text(t =>
            {
                t.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                t.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                t.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }

    static void ComposeTable(IContainer c, List<(Employee Employee, PayrollResult Result)> rows)
    {
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
                h.Cell().Element(BrandHeaderCell).Text("Nombre");
                h.Cell().Element(BrandHeaderCell).Text("Apellido");
                h.Cell().Element(BrandHeaderCell).AlignRight().Text("Bruto");
                h.Cell().Element(BrandHeaderCell).AlignRight().Text("Seg. Social");
                h.Cell().Element(BrandHeaderCell).AlignRight().Text("Seg. Educativo");
                h.Cell().Element(BrandHeaderCell).AlignRight().Text("ISR");
                h.Cell().Element(BrandHeaderCell).AlignRight().Text("Neto");
            });

            var index = 0;
            foreach (var (e, r) in rows.Select(x => (x.Employee, x.Result)))
            {
                var bg = index % 2 == 0 ? Colors.White : ZebraLight;

                t.Cell().Element(c => DataCell(c, bg)).Text(e.Nombre);
                t.Cell().Element(c => DataCell(c, bg)).Text(e.Apellido);
                t.Cell().Element(c => DataCellRight(c, bg)).Text(r.Gross.ToString("N6", CultureInfo.InvariantCulture));
                t.Cell().Element(c => DataCellRight(c, bg)).Text(r.SocialSecurity.ToString("N6", CultureInfo.InvariantCulture));
                t.Cell().Element(c => DataCellRight(c, bg)).Text(r.EducationalInsurance.ToString("N6", CultureInfo.InvariantCulture));
                t.Cell().Element(c => DataCellRight(c, bg)).Text(r.IncomeTax.ToString("N6", CultureInfo.InvariantCulture));
                t.Cell().Element(c => DataCellRightNet(c, bg)).Text(r.Net.ToString("N6", CultureInfo.InvariantCulture));

                index++;
            }

            // Totals row
            t.Cell().ColumnSpan(2).Element(TotalsCell).Text("TOTALES").SemiBold();
            t.Cell().Element(TotalsCellRight).Text(rows.Sum(x => x.Result.Gross).ToString("N6", CultureInfo.InvariantCulture)).SemiBold();
            t.Cell().Element(TotalsCellRight).Text(rows.Sum(x => x.Result.SocialSecurity).ToString("N6", CultureInfo.InvariantCulture)).SemiBold();
            t.Cell().Element(TotalsCellRight).Text(rows.Sum(x => x.Result.EducationalInsurance).ToString("N6", CultureInfo.InvariantCulture)).SemiBold();
            t.Cell().Element(TotalsCellRight).Text(rows.Sum(x => x.Result.IncomeTax).ToString("N6", CultureInfo.InvariantCulture)).SemiBold();
            t.Cell().Element(TotalsCellRight).Text(rows.Sum(x => x.Result.Net).ToString("N6", CultureInfo.InvariantCulture)).SemiBold();
        });
    }

    static IContainer BrandHeaderCell(IContainer c) => c
        .Background(BrandBlue)
        .PaddingVertical(6).PaddingHorizontal(8)
        .BorderBottom(0.5f).BorderColor(BorderColor)
        .DefaultTextStyle(x => x.FontColor(Colors.White).SemiBold().FontSize(9));

    static IContainer DataCell(IContainer c, Color bg) => c
        .Background(bg)
        .PaddingVertical(6).PaddingHorizontal(8)
        .BorderBottom(0.5f).BorderColor(BorderColor)
        .DefaultTextStyle(x => x.FontSize(9));

    static IContainer DataCellRight(IContainer c, Color bg) => c
        .Background(bg)
        .PaddingVertical(6).PaddingHorizontal(8)
        .BorderBottom(0.5f).BorderColor(BorderColor)
        .DefaultTextStyle(x => x.FontSize(9))
        .AlignRight();

    static IContainer DataCellRightNet(IContainer c, Color bg) => c
        .Background(bg)
        .PaddingVertical(6).PaddingHorizontal(8)
        .BorderBottom(0.5f).BorderColor(BorderColor)
        .DefaultTextStyle(x => x.FontSize(9).FontColor(GreenDark).SemiBold())
        .AlignRight();

    static IContainer TotalsCell(IContainer c) => c
        .Background(TotalsBg)
        .PaddingVertical(6).PaddingHorizontal(8)
        .DefaultTextStyle(x => x.FontSize(9).SemiBold());

    static IContainer TotalsCellRight(IContainer c) => c
        .Background(TotalsBg)
        .PaddingVertical(6).PaddingHorizontal(8)
        .DefaultTextStyle(x => x.FontSize(9).SemiBold())
        .AlignRight();
}
