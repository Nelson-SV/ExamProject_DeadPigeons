using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.TransferModels.Requests;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.BalanceDto;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/play")]
    public class PlayController : ControllerBase
    {
        private readonly IPlayService _service;

       
        public PlayController(IPlayService service)
        {
            _service = service;
        }


        [HttpPost]
        [Route("gameTickets")]
        [Authorize(Roles = Role.Player)]
        public async Task<ActionResult<(CurrentBalanceDto, List<AutomatedTicketsDto>)>> CreateGameTicket([FromBody] List<CreateTicketDto > tickets)
        {
            var (updatedBalance, automatedTicketsDtos) = await _service.CreateGameTicket(tickets);
            return Ok(new
                {
                    currentBalance = updatedBalance,
                    automatedTickets = automatedTicketsDtos
                });
            
        }
        
        [HttpGet]
        [Route("")]
        [Authorize(Roles = Role.Player)]
        public ActionResult<List<AutomatedTicketsDto>> GetAutomatedTickets([FromQuery] string userId)
        {
            var tickets = _service.GetAutomatedTickets(userId);
            return Ok(tickets);
                
        }

        [HttpGet]
        [Route("checkIsAllowedToPlay")]
        [Authorize(Roles = Role.Player)]

        public async Task<ActionResult<GameIdDto>> CheckIsAllowedToPlay()
        {
            var gameId = await _service.CheckIsAllowedToPlay(); 
            return Ok(gameId);
            
            
        }
        
        
        [HttpDelete]
        [Route("deleteAutomatedTicket")]
        [Authorize(Roles = Role.Player)]
        public async Task<ActionResult> DeleteAutomatedTicket([FromBody] AutomatedTicketsDto ticket)
        {
            if (ticket.Guid == Guid.Empty)
            {
                return BadRequest("Ticket Id is required.");
            }

            try
            {
                var resultMessage = await _service.DeleteAutomatedTicket(ticket);  
                return Ok(new { message = resultMessage });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message});
            }
        }

        [HttpPut]
        [Route("updateAutomatedTicketStatus")]
        [Authorize(Roles = Role.Player)]
        public async Task<ActionResult> UpdateAutomatedTicketStatus([FromBody] UpdateAutomatedTicketStatusRequest request)
        {
            if (request == null || request.AutomatedTicket.Guid == Guid.Empty)
            {
                return BadRequest("Invalid data to update.");
            }

            try
            {
                var resultMessage = await _service.UpdateAutomatedTicketStatus(request.AutomatedTicket, request.IsActive);  
                return Ok(new { message = resultMessage });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}