using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    GetProductsUseCase getProductsUseCase,
    CreateProductUseCase createProductUseCase,
    UpdateProductUseCase updateProductUseCase,
    DeleteProductUseCase deleteProductUseCase) : ControllerBase
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductRequest request)
    {
        var result = await updateProductUseCase.ExecuteAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await deleteProductUseCase.ExecuteAsync(id);
        return NoContent();
    }
}
