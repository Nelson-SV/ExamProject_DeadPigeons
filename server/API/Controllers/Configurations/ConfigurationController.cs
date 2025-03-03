using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.TransferModels.Responses.Configurations;


namespace Api.Controllers.Configurations;

[ApiController]
[Route("/configurations")]
public class ConfigurationController : ControllerBase
{
    private ConfigurationService _configuration;

    public ConfigurationController(ConfigurationService configuration)
    {
        _configuration = configuration;
    }
 [AllowAnonymous]
     [Route("/ticketPrices")]
     public async Task<ActionResult<Dictionary<int, TicketPriceDto>>> GetTicketPrices()
     {
         var ticketPrices = await _configuration.GetTicketPrices();
         return Ok(ticketPrices);
     }
 [AllowAnonymous]
     [Route("/topUpPrices")] 
 public async Task<ActionResult<List<int>>> GetTopUpPrices()
     {
        var topUpValue = await _configuration.GetTopUpValue();
         return Ok(topUpValue);
     }
}