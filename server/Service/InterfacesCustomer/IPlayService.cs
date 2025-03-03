using DataAccess.QuerryModels.Balance;
using Service.TransferModels.Requests;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.BalanceDto;
using AutomatedTicketsDto = Service.TransferModels.Responses.AutomatedTicketsDto;

namespace Service;


public interface IPlayService
{
    Task<(CurrentBalanceDto, List<AutomatedTicketsDto>)> CreateGameTicket(List<CreateTicketDto> tickets);
    List<AutomatedTicketsDto> GetAutomatedTickets(string userId);
    Task<GameIdDto> CheckIsAllowedToPlay();
    Task<string> UpdateAutomatedTicketStatus(AutomatedTicketsDto automatedTicket, bool isActive);
    Task<string> DeleteAutomatedTicket(AutomatedTicketsDto ticketId);
}