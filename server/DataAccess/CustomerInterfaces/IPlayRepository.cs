using DataAccess.Models;

namespace DataAccess.CustomerInterfaces;

public interface IPlayRepository

{
    Task<(Balance, List<AutomatedTicket>)> ManageTickets(List<GameTicket> gameTickets, int balance, string userId,
        List<AutomatedTicket> automatedTickets, string gameId, int total);

    Task CreateGameTicket(List<GameTicket> gameTicket);
    Task UpdateRevenue(int totalPrice, string gameId);
    Task<Balance> UpdateBalance(int balance, string userId);
    Task<List<AutomatedTicket>> CreateAutomatedTicket(List<AutomatedTicket> automatedTickets);
    List<AutomatedTicket> GetAutomatedTickets(string userId);
    Task DeleteAutomatedTicket(Guid ticketId);
    Task UpdateAutomatedTicketStatus(Guid ticketId, bool isActive, AutomatedTicket automatedTicket);
    Task<int> GetBalance(string userId);
    Task<Game> CheckIsAllowedToPlay();
    Task<Game?> GetActiveGame();
    Task<bool> IsAutomatedTicketBoughtForCurrentWeek(AutomatedTicket ticket, string gameId);

   
}