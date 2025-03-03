using DataAccess.Models;

namespace DataAccess.CustomerInterfaces;

public interface IUserRepository
{
    List<GameTicket> GetUserGameTicketsHistory(string userId, int page, int pageSize, out int totalTickets);
    Game GetGameById(string gameId);
    WeeklyTicketSummary? GetSummaryByGameId(string gameId);
} 