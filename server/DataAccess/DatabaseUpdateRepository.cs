using Common.ErrorMessages;
using DataAccess.Models;
using Microsoft.Extensions.Logging;
using ApplicationException = Common.AplicationExceptions.ApplicationException;

namespace DataAccess;
using System.Linq; 
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


public class DatabaseUpdateRepository : IDataBaseUpdateRepository
{
    private AppDbContext _appDbContext;
    
    private readonly ILogger<DatabaseUpdateRepository> _logger;

    public DatabaseUpdateRepository(AppDbContext appDbContext, ILogger<DatabaseUpdateRepository> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task UpdateGameState()
    {
        try
        {
            var game = await _appDbContext.Games
                .Where(g => g.Status == true)  
                .FirstOrDefaultAsync();  

            if (game == null)
            {
                throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.GameTicketStatusAutomisation));
            }
        
            game.Status = false;
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }
    
}