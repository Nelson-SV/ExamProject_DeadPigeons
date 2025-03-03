using Service.TransferModels.Responses;

namespace Service.TransferModels.Requests
{
    public class UpdateAutomatedTicketStatusRequest
    {
        public AutomatedTicketsDto AutomatedTicket { get; set; }
        public bool IsActive { get; set; }
    }
}