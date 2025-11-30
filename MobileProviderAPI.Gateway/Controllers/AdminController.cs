using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileProviderAPI.Models.DTOs;
using MobileProviderAPI.Services;
using System.Security.Claims;

namespace MobileProviderAPI.Gateway.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IBillService _billService;

    public AdminController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpPost("addbill")]
    public async Task<ActionResult<TransactionResponse>> AddBill([FromBody] AddBillRequest request)
    {
        var result = await _billService.AddBillAsync(request);
        
        if (result.Status == "Error")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("addbillbatch")]
    public async Task<ActionResult<TransactionResponse>> AddBillBatch(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new TransactionResponse
            {
                Status = "Error",
                ErrorMessage = "No file uploaded"
            });
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new TransactionResponse
            {
                Status = "Error",
                ErrorMessage = "File must be a CSV file"
            });
        }

        using var stream = file.OpenReadStream();
        var result = await _billService.AddBillsBatchAsync(stream);
        
        if (result.Status == "Error")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

