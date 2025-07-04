using ClosedXML.Excel;
using MediatR;

namespace Navisaf.Application.Features.Orders.Queries;

public sealed class DownloadExportExcelQuery : IRequest<FileDto>;

public sealed class DownloadExportExcelQueryHandler(IMediator mediator) : IRequestHandler<DownloadExportExcelQuery, FileDto>
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private const string FileName = "OrdersReport.xlsx";
    public async Task<FileDto> Handle(DownloadExportExcelQuery request, CancellationToken cancellationToken)
    {
        var orders = await mediator.Send(new ReportQuery(), cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders Report");
        worksheet.Cell(1, 1).Value = "Period";
        worksheet.Cell(1, 2).Value = "Customer Name";
        worksheet.Cell(1, 3).Value = "Total Orders";

        for (var i = 0; i < orders.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = orders[i].Period;
            worksheet.Cell(i + 2, 2).Value = orders[i].CustomerName;
            worksheet.Cell(i + 2, 3).Value = orders[i].TotalOrders;
        }
        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;
        return new FileDto(stream.ToArray(), FileName, ContentType);
    }
}

public class FileDto(byte[] content, string fileName, string contentType)
{
    public byte[] Content { get; set; } = content;
    public string FileName { get; set; } = fileName;
    public string ContentType { get; set; } = contentType;
}