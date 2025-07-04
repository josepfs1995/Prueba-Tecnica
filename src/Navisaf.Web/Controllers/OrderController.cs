using MediatR;
using Microsoft.AspNetCore.Mvc;
using Navisaf.Application.Features.Orders.Command;
using Navisaf.Application.Features.Orders.Queries;
using Navisaf.Application.Features.Products.Queries;

namespace Navisaf.Web.Controllers;

public class OrderController(IMediator mediator) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Get(string customerName)
    {
        var response = await mediator.Send(new OrderListQuery(customerName));
        return PartialView("_OrderList", response);
    }
    public async Task<IActionResult> Report()
    {
        var response = await mediator.Send(new ReportQuery());
        return View(response);
    }
    public async Task<IActionResult> DownloadExcel()
    {
        var response = await mediator.Send(new DownloadExportExcelQuery());
        return File(response.Content, response.ContentType, response.FileName);
    }
    [Route("Detail/{orderId}")]
    public async Task<IActionResult> Detail(Guid orderId)
    {
        try
        {
            var response = await mediator.Send(new OrderByIdQuery(orderId));
            return View(response);
        }
        catch (KeyNotFoundException)
        {
            return RedirectToAction("Error", "Home");
        }
    }
    public async Task<IActionResult> Create()
    {
        var products = await mediator.Send(new ProductListQuery());
        ViewBag.Products = products;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        try
        {
            await mediator.Send(command);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)   //HACK:  Pude usar un tipo de excepción más específico para FluentValidation, pero no lo hice para simplificar el ejemplo.
        {
            return BadRequest(ex.Message);
        }
    }
}
