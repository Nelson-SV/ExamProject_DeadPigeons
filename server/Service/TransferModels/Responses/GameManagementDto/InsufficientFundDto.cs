using DataAccess.Models;

namespace Service.TransferModels.Responses.GameManagementDto;

public class InsufficientFundDto
{
    public string UserName { get; set; } = "";
    public string? UserEmail { get; set; }
    public int BalanceValue { get; set; }
    public List<GameTicketDto> FailedToInsertTickets { get; set; } = new List<GameTicketDto>();
    public DateTime CurrentGameDate { get; set; }
}