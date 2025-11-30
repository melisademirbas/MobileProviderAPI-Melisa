using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobileProviderAPI.Data.Repositories;
using MobileProviderAPI.Models.DTOs;
using MobileProviderAPI.Services;

namespace MobileProviderAPI.Gateway.Controllers;

[ApiController]
[Route("api/mobileapp")]
[Authorize]
public class MobileAppController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IApiCallLogRepository _apiCallLogRepository;
    private readonly ISubscriberRepository _subscriberRepository;
    private const int MAX_CALLS_PER_DAY = 3;

    public MobileAppController(
        IBillService billService,
        IApiCallLogRepository apiCallLogRepository,
        ISubscriberRepository subscriberRepository)
    {
        _billService = billService;
        _apiCallLogRepository = apiCallLogRepository;
        _subscriberRepository = subscriberRepository;
    }

    [HttpGet("querybill")]
    public async Task<ActionResult<QueryBillResponse>> QueryBill([FromQuery] string subscriberNo, [FromQuery] string month)
    {
        // Rate limiting: 3 calls per subscriber per day
        var callCount = await _apiCallLogRepository.GetApiCallCountTodayAsync(subscriberNo, "/api/mobileapp/querybill");
        if (callCount >= MAX_CALLS_PER_DAY)
        {
            return StatusCode(429, new { message = $"Rate limit exceeded. Maximum {MAX_CALLS_PER_DAY} calls per subscriber per day." });
        }

        // Log API call
        var subscriber = await _subscriberRepository.GetBySubscriberNoAsync(subscriberNo);
        await _apiCallLogRepository.LogApiCallAsync(new MobileProviderAPI.Models.ApiCallLog
        {
            SubscriberId = subscriber?.Id,
            SubscriberNo = subscriberNo,
            Endpoint = "/api/mobileapp/querybill",
            Method = "GET",
            CallDate = DateTime.UtcNow
        });

        var result = await _billService.QueryBillAsync(subscriberNo, month);
        if (result == null)
        {
            return NotFound(new { message = "Bill not found" });
        }

        return Ok(result);
    }

    [HttpGet("querybilldetailed")]
    public async Task<ActionResult<QueryBillDetailedResponse>> QueryBillDetailed(
        [FromQuery] string subscriberNo,
        [FromQuery] string month,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _billService.QueryBillDetailedAsync(subscriberNo, month, pageNumber, pageSize);
        if (result == null)
        {
            return NotFound(new { message = "Bill not found" });
        }

        return Ok(result);
    }
}

