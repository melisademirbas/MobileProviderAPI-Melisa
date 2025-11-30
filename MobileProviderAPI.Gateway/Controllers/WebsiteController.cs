using Microsoft.AspNetCore.Mvc;
using MobileProviderAPI.Models.DTOs;
using MobileProviderAPI.Services;

namespace MobileProviderAPI.Gateway.Controllers;

[ApiController]
[Route("api/website")]
public class WebsiteController : ControllerBase
{
    private readonly IBillService _billService;

    public WebsiteController(IBillService billService)
    {
        _billService = billService;
    }

    [HttpPost("paybill")]
    public async Task<ActionResult<PayBillResponse>> PayBill([FromBody] PayBillRequest request)
    {
        var result = await _billService.PayBillAsync(request.SubscriberNo, request.Month, request.Amount);
        
        if (result.PaymentStatus == "Error")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}

