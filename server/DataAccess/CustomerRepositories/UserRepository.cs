using Common.ErrorMessages;
using DataAccess.CustomerInterfaces;
using DataAccess.Models;

namespace DataAccess.CustomerRepositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public List<GameTicket> GetUserGameTicketsHistory(string userId, int page, int pageSize, out int totalTickets)
    {
        totalTickets = _context.GameTickets
            .Count(t => t.UserId == userId);

        var activeGameStartDate = _context.Games
            .OrderByDescending(g => g.StartDate)
            .FirstOrDefault()?.StartDate ?? DateTime.UtcNow;

        return _context.GameTickets
            .Where(t => t.UserId == userId && t.PurchaseDate <= activeGameStartDate)
            .OrderByDescending(t => t.PurchaseDate) 
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
    
    public Game GetGameById(string gameId)
    {
        var game = _context.Games.FirstOrDefault(g => g.Guid == gameId);
        if (game == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.GameRetrieveError));
        }
        return game;
    }

    public WeeklyTicketSummary? GetSummaryByGameId(string gameId)
    {
        return _context.WeeklyTicketSummaries.FirstOrDefault(s => s.GameId == gameId);
    }

    
}