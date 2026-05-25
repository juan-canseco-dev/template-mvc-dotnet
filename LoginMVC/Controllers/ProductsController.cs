using LoginMVC.Services.Interfaces;
using LoginMVC.Shared.DataTable;
using LoginMVC.Shared.Results;
using LoginMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LoginMVC.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService productService;

    public ProductsController(IProductService productService)
    {
        this.productService = productService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetTableData(CancellationToken cancellationToken)
    {
        var draw = Request.Form["draw"].FirstOrDefault();
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
        var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        var columnName = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();

        int pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
        int skip = string.IsNullOrEmpty(start) ? 0 : Convert.ToInt32(start);
        int pageNumber = (skip / pageSize) + 1;

        string orderByField = "id";
        if (!string.IsNullOrEmpty(columnName))
        {
            orderByField = columnName.ToLower();
        }

        var request = new GetProductsPageViewModel
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Name = searchValue,
            OrderBy = orderByField,
            SortOrder = sortDirection
        };

        var result = await productService.GetPaginatedAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Json(new
            {
                draw = draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = Array.Empty<ProductViewModel>(),
                error = "We couldn't load the product list right now. Please try refreshing the page."
            });
        }

        var response = new DataTablesResponse<ProductViewModel>
        {
            Draw = string.IsNullOrEmpty(draw) ? 0 : Convert.ToInt32(draw),
            RecordsTotal = result.Value.TotalCount,
            RecordsFiltered = result.Value.TotalCount,
            Data = result.Value.Items
        };

        return Json(response);
    }

    [HttpGet]
    public IActionResult CreateModal()
    {
        return PartialView("_CreateModal", new CreateProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateModal(CreateProductViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(Result.Failure(new Error("ValidationError", string.Join(" ", errors))));
        }

        var result = await productService.CreateAsync(model, cancellationToken);
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> EditModal(int id, CancellationToken cancellationToken)
    {
        var result = await productService.GetByIdAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest("We couldn't find the product you're trying to edit. It might have been deleted.");
        }

        var updateModel = new UpdateProductViewModel
        {
            ProductId = result.Value.Id,
            Name = result.Value.Name,
            PurchasePrice = result.Value.PurchasePrice,
            SalePrice = result.Value.SalePrice,
            Quantity = result.Value.Quantity
        };

        return PartialView("_EditModal", updateModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditModal(UpdateProductViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(Result.Failure(new Error("ValidationError", string.Join(" ", errors))));
        }

        var result = await productService.UpdateAsync(model, cancellationToken);
        return Json(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await productService.DeleteAsync(id, cancellationToken);
        return Json(result);
    }
}