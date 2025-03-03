using DataAccess.Models;

namespace DataAccess.QuerryModels.Admin;

public class InsufficientFundQto
{
    public string UserName { get; set; } = "";
    public string? UserEmail { get; set; }
    public int BalanceValue { get; set; }
    public List<GameTicket> UninsertedTickets { get; set; } = new List<GameTicket>();
    public DateTime CurrentGameDate { get; set; }
}