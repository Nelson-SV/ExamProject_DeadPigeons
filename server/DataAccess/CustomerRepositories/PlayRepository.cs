using Common.ErrorMessages;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;
using DataAccess.CustomerInterfaces;
using Microsoft.Extensions.Logging;


namespace DataAccess.CustomerRepositories
{
    public class PlayRepository : IPlayRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PlayRepository> _logger;
   

    public PlayRepository(AppDbContext context, ILogger<PlayRepository> logger)
    {
        _context = context;
        _logger = logger;
       
    }
    public async Task UpdateRevenue(int totalPrice, string gameId)
    {
        var game =  await  _context.Games
                .FirstOrDefaultAsync(g=>g.Guid == gameId);
            
        if (game == null)
        {
                _logger.LogError($"Game with Guid: {gameId} not found");
                throw new Exception(ErrorMessages.GetMessage(ErrorCode.GameIdNotFound));
 
        }
            
        game.Revenue += totalPrice;
            
        _context.Update(game);
            
        await _context.SaveChangesAsync();
    }

     public async Task<(Balance, List<AutomatedTicket>)> ManageTickets(List<GameTicket> gameTickets, int balance, string userId, List<AutomatedTicket> automatedTickets, string gameId, int totalPrice)
    {
        await using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                var updatedBalance = await UpdateBalance(balance, userId);

                await CreateGameTicket(gameTickets);
                
                List<AutomatedTicket> automatedTicketsList = await CreateAutomatedTicket(automatedTickets);
                
                await UpdateRevenue(totalPrice, gameId);

                await transaction.CommitAsync();
                
                return (updatedBalance, automatedTicketsList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, ErrorMessages.GetMessage(ErrorCode.InsertingTicketsFailed));
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InsertingTicketsFailedCustomer), ex);
                
            }
        }
    }

    public async Task CreateGameTicket(List<GameTicket> gameTickets)
    {
        await _context.GameTickets.AddRangeAsync(gameTickets);
        await _context.SaveChangesAsync();
    }
    

    public async Task<Balance> UpdateBalance(int balance, string userId)
    {

        var userBalance = await _context.Balances
            .FirstOrDefaultAsync(b => b.UserId == userId);

        if (userBalance == null)
        {
            _logger.LogError(ErrorMessages.GetMessage(ErrorCode.BalanceEmpty));
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.BalanceEmpty));
        }

        userBalance.Value = balance;
        _context.Update(userBalance);
        await _context.SaveChangesAsync();

        return userBalance;
    }

    public async Task<List<AutomatedTicket>> CreateAutomatedTicket(List<AutomatedTicket> automatedTickets)
    {

        await _context.AutomatedTickets.AddRangeAsync(automatedTickets);
        await _context.SaveChangesAsync();
        return automatedTickets;
    }

    public List<AutomatedTicket> GetAutomatedTickets(string userId)
    {
        try
        {
            var automatedTickets = _context.AutomatedTickets
                .Where(t => t.UserId == userId)
                .ToList();

            return automatedTickets;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessages.GetMessage(ErrorCode.FailedToRetrieveAutomatedTickets));
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.NoTicketsFound));
        }

       
    }

    public async Task<int> GetBalance(string userId)
    {
        var balance = await _context.Balances
            .Where(t => t.UserId == userId)
            .Select(t => t.Value)
            .FirstOrDefaultAsync();
        return balance;
    }


    public async Task<Game> CheckIsAllowedToPlay()
    {
        var game = await _context.Games
                .Where(g => g.Status == true)  
                .FirstOrDefaultAsync();  

        if (game == null)
        {
            throw new ApplicationException((ErrorMessages.GetMessage(ErrorCode.GameTicketStatusAutomisation)));
        }

        return game;

    }


        public async Task DeleteAutomatedTicket(Guid ticketId)
        {
            var ticket = await _context.AutomatedTickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.ErrorDeletingTicket));
            }

            _context.AutomatedTickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAutomatedTicketStatus(Guid ticketId, bool isActive, AutomatedTicket automatedTicket)
        {
            var ticket = await _context.AutomatedTickets.FindAsync(ticketId);
            if (ticket == null) return;

            ticket.IsActive = isActive;
            
            if (ticketId != automatedTicket.Guid)
            {
                _context.AutomatedTickets.Remove(ticket);
                await _context.SaveChangesAsync();
                return;
            }

            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        
        public async Task<Game?> GetActiveGame()
        {
            return await _context.Games
                .Where(g => g.Status == true)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsAutomatedTicketBoughtForCurrentWeek(AutomatedTicket ticket, string gameId)
        {
            return await _context.GameTickets
                .AnyAsync(gt => gt.GUID == ticket.Guid && gt.GameId == gameId);
        }
        
    }
}
