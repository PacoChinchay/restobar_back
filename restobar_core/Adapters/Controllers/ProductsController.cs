using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    GetProductsUseCase getProductsUseCase,
    CreateProductUseCase createProductUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetActive()
    {
        return Ok(await getProductsUseCase.ExecuteAsync());
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        return Ok(await getProductsUseCase.ExecuteAllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request)
    {
        var result = await createProductUseCase.ExecuteAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
