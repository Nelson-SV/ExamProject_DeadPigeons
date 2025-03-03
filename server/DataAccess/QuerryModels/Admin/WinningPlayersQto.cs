using Common.SharedModels;
using DataAccess.Models;

namespace DataAccess.QuerryModels.Admin;

public class WinningPlayersQto
{
    
    public List<GameTicket>? WinningTickets { get; set; }
    public Pagination? Pagination { get; set; }
}