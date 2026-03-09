using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of IPdfGenerator using QuestPDF library.
/// </summary>
public class QuestPdfGenerator : IPdfGenerator
{
    public QuestPdfGenerator()
    {
        // Set QuestPDF license to Community
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public void GenerateSalesReport(
        int totalOrders,
        decimal totalRevenue,
        decimal averageOrderValue,
        IEnumerable<OrderStatusSummary> statusSummaries,
        IEnumerable<ProductSalesSummary> topProducts,
        IEnumerable<DailySalesSummary> dailySales,
        string filePath)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Prompt Store - Sales Report").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"{DateTime.Now:f}").FontSize(10).Italic();
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    // Overview Section
                    col.Item().PaddingBottom(5).Text("Overview").FontSize(14).SemiBold().Underline();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(150);
                            columns.RelativeColumn();
                        });

                        table.Cell().Text("Total Orders:");
                        table.Cell().Text($"{totalOrders}");

                        table.Cell().Text("Total Revenue:");
                        table.Cell().Text($"R{totalRevenue:F2}");

                        table.Cell().Text("Average Order Value:");
                        table.Cell().Text($"R{averageOrderValue:F2}");
                    });

                    col.Item().PaddingVertical(10);

                    // Orders by Status
                    col.Item().PaddingBottom(5).Text("Orders by Status").FontSize(14).SemiBold().Underline();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Status");
                            header.Cell().Element(CellStyle).Text("Count");
                            header.Cell().Element(CellStyle).Text("Revenue");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var status in statusSummaries)
                        {
                            table.Cell().Element(RowStyle).Text($"{status.Status}");
                            table.Cell().Element(RowStyle).Text($"{status.Count}");
                            table.Cell().Element(RowStyle).Text($"R{status.Total:F2}");
                        }

                        static IContainer RowStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });

                    col.Item().PaddingVertical(10);

                    // Top Selling Products
                    col.Item().PaddingBottom(5).Text("Top Selling Products").FontSize(14).SemiBold().Underline();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Product");
                            header.Cell().Element(CellStyle).Text("Qty Sold");
                            header.Cell().Element(CellStyle).Text("Revenue");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var product in topProducts)
                        {
                            table.Cell().Element(RowStyle).Text($"{product.ProductId}");
                            table.Cell().Element(RowStyle).Text($"{product.ProductName}");
                            table.Cell().Element(RowStyle).Text($"{product.TotalQuantitySold}");
                            table.Cell().Element(RowStyle).Text($"R{product.TotalRevenue:F2}");
                        }

                        static IContainer RowStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });

                    col.Item().PaddingVertical(10);

                    // Daily Sales
                    col.Item().PaddingBottom(5).Text("Daily Sales Breakdown").FontSize(14).SemiBold().Underline();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Date");
                            header.Cell().Element(CellStyle).Text("Orders");
                            header.Cell().Element(CellStyle).Text("Revenue");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var day in dailySales)
                        {
                            table.Cell().Element(RowStyle).Text($"{day.Date:yyyy-MM-dd}");
                            table.Cell().Element(RowStyle).Text($"{day.OrderCount}");
                            table.Cell().Element(RowStyle).Text($"R{day.Revenue:F2}");
                        }

                        static IContainer RowStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });
                });

                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        });

        document.GeneratePdf(filePath);
    }

    public void GenerateInventoryReport(
        IEnumerable<Product> products,
        IEnumerable<Product> lowStockProducts,
        IEnumerable<Product> outOfStockProducts,
        string filePath)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Prompt Store - Inventory Report").FontSize(20).SemiBold().FontColor(Colors.Green.Medium);
                        col.Item().Text($"{DateTime.Now:f}").FontSize(10).Italic();
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    // Out of Stock Section (Alert)
                    if (outOfStockProducts.Any())
                    {
                        col.Item().Background(Colors.Red.Lighten4).Padding(10).Column(innerCol =>
                        {
                            innerCol.Item().Text("CRITICAL: Out of Stock Items").FontSize(12).SemiBold().FontColor(Colors.Red.Medium);
                            foreach (var p in outOfStockProducts)
                            {
                                innerCol.Item().Text($"- {p.Name} (ID: {p.Id})");
                            }
                        });
                        col.Item().PaddingVertical(10);
                    }

                    // Low Stock Section (Warning)
                    if (lowStockProducts.Any(p => p.Stock > 0))
                    {
                        col.Item().Background(Colors.Yellow.Lighten4).Padding(10).Column(innerCol =>
                        {
                            innerCol.Item().Text("WARNING: Low Stock Items").FontSize(12).SemiBold().FontColor(Colors.Orange.Medium);
                            foreach (var p in lowStockProducts.Where(p => p.Stock > 0))
                            {
                                innerCol.Item().Text($"- {p.Name} (ID: {p.Id}) - Current Stock: {p.Stock}");
                            }
                        });
                        col.Item().PaddingVertical(10);
                    }

                    // Full Inventory Table
                    col.Item().PaddingBottom(5).Text("Full Inventory List").FontSize(14).SemiBold().Underline();
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Name");
                            header.Cell().Element(CellStyle).Text("Category");
                            header.Cell().Element(CellStyle).Text("Price");
                            header.Cell().Element(CellStyle).Text("Stock");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var product in products)
                        {
                            table.Cell().Element(RowStyle).Text($"{product.Id}");
                            table.Cell().Element(RowStyle).Text($"{product.Name}");
                            table.Cell().Element(RowStyle).Text($"{product.Category}");
                            table.Cell().Element(RowStyle).Text($"R{product.Price:F2}");
                            
                            var stockCell = table.Cell().Element(RowStyle);
                            if (product.Stock == 0) stockCell.Text($"{product.Stock}").FontColor(Colors.Red.Medium).Bold();
                            else if (product.Stock <= 5) stockCell.Text($"{product.Stock}").FontColor(Colors.Orange.Medium).Bold();
                            else stockCell.Text($"{product.Stock}");
                        }

                        static IContainer RowStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });
                });

                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        });

        document.GeneratePdf(filePath);
    }
}
