using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileProviderAPI.Models.DTOs;
using MobileProviderAPI.Services;

namespace MobileProviderAPI.Gateway.Controllers;

[ApiController]
[Route("api/bankingapp")]
[Authorize]
public class BankingAppController : ControllerBase
{
    private readonly IBillService _billService;

    public BankingAppController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpGet("querybill")]
    public async Task<ActionResult<List<QueryBillResponse>>> QueryBill([FromQuery] string subscriberNo)
    {
        var result = await _billService.QueryUnpaidBillsAsync(subscriberNo);
        return Ok(result);
    }
}

