using Microsoft.AspNetCore.Mvc;
using restobar_core.Application.DTOs;
using restobar_core.Application.UseCases;

namespace restobar_core.Adapters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(GetProductsUseCase useCase) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetActive()
    {
        return Ok(await useCase.ExecuteAsync());
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        return Ok(await useCase.ExecuteAllAsync());
    }
}
