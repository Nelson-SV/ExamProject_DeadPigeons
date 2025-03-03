using Common.SharedModels;
using DataAccess.Models;
using DataAccess.QuerryModels.Admin;

namespace DataAccess.admin.GameManagementRepository;

public interface IGameManagementRepository
{
    Task<Game?> GetCurrentGame();
    Task<bool> ValidateSequenceRequest(string gameId);

    Task<ProcessedWinningSequenceResponseQto> ProcessWinningSequence(List<int> sequence , string gameId );
    Task<bool> ValidateGameAsync(string gameIdGuid);
    Task<WinningPlayersQto> GetWinningPlayers(string gameIdGuid,Pagination pagination,List<int> winningSequence);
    Task<IEnumerable<GameTicket>?> GetTicketsForPlayer(List<Guid>? playerTicketsIds);

    Task<IEnumerable<AutomatedTicket>> GetAutomatedTickets();

    Task<bool> UpdateBalance(int balanceValue,string userId);


    Task<List<Game>> FindGameByWeekAndYear(int gameRequestWeek, int gameRequestYear);
    Task<Tuple<List<Game>,Pagination>> GetGames(Pagination pagination);

    Task<Game?> GetGameById(string guid);
}