using Common;
using Common.ErrorMessages;
using Common.SharedModels;
using DataAccess.Models;
using DataAccess.QuerryModels.Admin;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Service;

namespace DataAccess.admin.GameManagementRepository;

public class GameManagementRepository : IGameManagementRepository


{
    private AppDbContext _appContext;


    public GameManagementRepository(AppDbContext appContext)
    {
        _appContext = appContext;
    }


    public async Task<Game?> GetCurrentGame()
    {
        var currentGameWeekAndYear = new WeekExtractor().GetWeekNumberAndYear(DateTime.UtcNow);
        var game = await _appContext.Games
            .Where(g => g.ExtractedNumbers == null)
            .FirstOrDefaultAsync<Game>();
        return game;
    }

    private WeekYear<int, int> getWeekYear(DateTime date)
    {
        var (weekNumber, year) = new WeekExtractor().GetWeekNumberAndYear(date);
        return new WeekYear<int, int>(weekNumber, year);
    }

    /// <summary>
    /// Validate if the game is present in the database
    /// </summary>
    /// <param name="gameId">the game id that needs to be validated</param>
    /// <returns>true is present otherwise false</returns>
    public async Task<bool> ValidateSequenceRequest(string gameId)
    {
        var game = await _appContext.Games.FindAsync(gameId);
        return game is { ExtractedNumbers: null };
    }


    /// <summary>
    /// Insert the winning sequence for the current game, and all the winning players to the winning players table,
    /// so they can be queried easier and with pagination.
    /// </summary>
    public async Task<ProcessedWinningSequenceResponseQto> ProcessWinningSequence(List<int> sequence, string gameId)
    {
        var response = new ProcessedWinningSequenceResponseQto();


        var game = await _appContext.Games.FindAsync(gameId);
        await using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            game!.ExtractedNumbers = sequence.ToArray();
            game.ExtractionDate = DateTime.UtcNow;
            _appContext.Update(game);


            var winningPlayersIds = await RetrieveWinningPlayers(gameId, sequence);
            var winningPlayers = winningPlayersIds
                .Select(userId => new WinningPlayers
                {
                    GameId = gameId,
                    UserId = userId
                })
                .ToList();
            _appContext.WinningPlayers.AddRange(winningPlayers);

            var createNewGame = new Game
            {
                Guid = Guid.NewGuid().ToString(),
                StartDate = DateTime.Now.ToUtc(),
                Status = true
            };
            await _appContext.Games.AddAsync(createNewGame);
            response.UninsertedTickets =
                await InsertAutomatedTicketsIntoTheCurrentGame(createNewGame.Guid, createNewGame.StartDate);
            await _appContext.SaveChangesAsync();
            await transaction.CommitAsync();
            response.Registered = true;
            response.Message = SuccessMessages.GetMessage(SuccessCode.InsertedWinningNumbers);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, e);
            await transaction.RollbackAsync();
            response.Registered = false;
            response.Message = ErrorMessages.GetMessage(ErrorCode.InsertedWinningSequence);
        }

        return response;
    }


    private async Task<List<string>> RetrieveWinningPlayers(string gameId, List<int> winningSequence)
    {
        var winningUserIds = await _appContext.GameTickets
            .Where(gt => gt.GameId == gameId && winningSequence.All(s => gt.Sequence.Contains(s)))
            .Select(gt => gt.UserId)
            .Distinct()
            .ToListAsync();
        return winningUserIds;
    }


    /// <summary>
    /// Validate if a game exists in the database
    /// </summary>
    public async Task<bool> ValidateGameAsync(string gameIdGuid)
    {
        var response = await _appContext.Games.FirstOrDefaultAsync((g) => g.Guid == gameIdGuid);
        return response != null;
    }

    /// <summary>
    /// Get winning players, for a game, params gameId=request gameId,pagination= pagination request object  
    /// </summary>
    public async Task<WinningPlayersQto> GetWinningPlayers(string gameId, Pagination pagination,
        List<int> winningSequence)
    {
        var response = new WinningPlayersQto();
        var skipCount = (pagination.CurrentPage - 1) * pagination.CurrentPageItems;

        var winningPlayersIds = _appContext.WinningPlayers
            .Where((wp) => wp.GameId == gameId);

        var currentPlayers = winningPlayersIds
            .Skip(skipCount)
            .Take(pagination.CurrentPageItems);

        var winningTicketsList = await _appContext.GameTickets
            .Where(gt => gt.GameId == gameId
                         && currentPlayers.Contains(new WinningPlayers { UserId = gt.UserId, GameId = gt.GameId })
                         && winningSequence.All(seq => gt.Sequence.Contains(seq))).Include(gt => gt.User)
            .ToListAsync();

        var totalItems = await winningPlayersIds.CountAsync();
        pagination.HasNext = totalItems - skipCount > 0;
        pagination.TotalItems = totalItems;

        if (pagination.HasNext)
        {
            pagination.NextPage = pagination.CurrentPage + 1;
        }

        response.WinningTickets = winningTicketsList;
        response.Pagination = pagination;

        return response;
    }

    /// <summary>
    ///  retrieve all the winning tickets  for a player
    /// </summary>
    public async Task<IEnumerable<GameTicket>?> GetTicketsForPlayer(List<Guid>? playerTicketsIds)
    {
        try
        {
            var tickets = await _appContext.GameTickets
                .Where(gt => playerTicketsIds!.Contains(gt.GUID))
                .ToListAsync();
            return tickets;
        }
        catch (Exception e)
        {
            return null;
        }
    }


    private async Task<List<InsufficientFundQto>> InsertAutomatedTicketsIntoTheCurrentGame(
        string currentGameId,
        DateTime currentGameDate)
    {
        var uninsertedTickets = new Dictionary<string, InsufficientFundQto>();
        var insertedGameTickets = new List<GameTicket>();

        var ticketsGroupedByUser = await _appContext.AutomatedTickets.Where((at) => at.IsActive).Include(at => at.User)
            .GroupBy(at => at.UserId).ToListAsync();

        foreach (var group in ticketsGroupedByUser)
        {
            var userId = group.Key;
            var userTickets = group.ToList();
            var userBalance = await _appContext.Balances.FirstOrDefaultAsync(b => b.UserId == userId);

            if (userBalance == null)
            {
                throw new ApplicationException($"Balance not found for UserId: {userId}");
            }

            foreach (var ticket in userTickets)
            {
                if (AllowedToBUy(userBalance.Value, ticket.PriceValue))
                {
                    userBalance.Value = ComputeBalance(userBalance.Value, ticket.PriceValue);
                    insertedGameTickets.Add(MapFromAutomatedTicket(ticket, currentGameId));
                }
                else
                {
                    if (!uninsertedTickets.TryGetValue(userId, out var insufficientFundQto))
                    {
                        insufficientFundQto = new InsufficientFundQto
                        {
                            UserName = ticket.User.UserName!,
                            UserEmail = ticket.User.Email,
                            BalanceValue = userBalance.Value,
                            CurrentGameDate = currentGameDate,
                            UninsertedTickets = new List<GameTicket>()
                        };
                        uninsertedTickets[userId] = insufficientFundQto;
                    }

                    insufficientFundQto.UninsertedTickets.Add(MapFromAutomatedTicket(ticket, currentGameId));
                }
            }

            _appContext.Balances.Update(userBalance);
        }

        await _appContext.GameTickets.AddRangeAsync(insertedGameTickets);

        return uninsertedTickets.Values.ToList();
    }


    public async Task<IEnumerable<AutomatedTicket>> GetAutomatedTickets()
    {
        var automatedTickets = await _appContext.AutomatedTickets
            .ToListAsync();
        return automatedTickets;
    }

    public async Task<bool> UpdateBalance(int balanceValue, string userId)
    {
        var balance = await _appContext.Balances.FirstOrDefaultAsync((b) => b.UserId == userId);
        if (balance != null)
        {
            balance.Value = balanceValue;
            _appContext.Update(balance);
            await _appContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<List<Game>> FindGameByWeekAndYear(int gameRequestWeek, int gameRequestYear)
    {
        var query = $@"
        SELECT * 
        FROM ""Game""
        WHERE EXTRACT(WEEK FROM ""StartDate"") = {gameRequestWeek}
          AND EXTRACT(YEAR FROM ""StartDate"") = {gameRequestYear}";

        var result = await _appContext.Games
            .FromSqlRaw(query).OrderByDescending((g)=>g.StartDate)
            .ToListAsync();

        return result;
    }


    /// <summary>
    /// Retrieves the games from the database
    /// </summary>
    /// <param name="pagination">The pagination object that holds information about how the games should be paginated </param>
    /// <returns>A list of game objects</returns>
    public async Task<Tuple<List<Game>, Pagination>> GetGames(Pagination pagination)
    {
        var newPagination = new Pagination
        {
            CurrentPage = pagination.CurrentPage,
            CurrentPageItems = pagination.CurrentPageItems
        };

        var skipCount = SkipCount(pagination);

        var games = _appContext.Games;

        var currentGames = await games.OrderByDescending(g => g.StartDate)
            .Skip(skipCount)
            .Take(pagination.CurrentPageItems)
            .ToListAsync();


        var totalItems = await games.CountAsync();


        newPagination.TotalItems = totalItems;
        newPagination.HasNext = totalItems > 0;
        newPagination.CurrentPage = pagination.CurrentPage + 1;
        newPagination.NextPage = pagination.NextPage + 1;
        return new Tuple<List<Game>, Pagination>(currentGames, newPagination);
    }

    /// <summary>
    /// Get game info by id
    /// </summary>
    /// <param name="guid">Id of the game to be retrieved</param>
    /// <returns>Game object or null</returns>
    public async Task<Game?> GetGameById(string guid)
    {
        var result = await _appContext.Games.FirstOrDefaultAsync((g) => g.Guid == guid);
        return result;
    }

    private static int SkipCount(Pagination pagination)
    {
        var skipCount = (pagination.CurrentPage - 1) * pagination.CurrentPageItems;
        return skipCount;
    }


    /// <summary>
    /// Retrieve the winning players for a game id
    /// </summary>
    /// <param name="gameId">The id of a game </param>
    /// <param name="pagination">Necessary  for the total pages that will be displayed</param>
    /// <returns>A list of users , and the pagination with the total pages</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Tuple<List<User>, Pagination>> GetWinningPlayersForAGameId(string gameId, Pagination pagination)
    {
        var newPagination = new Pagination();
        var winningplayers = _appContext.WinningPlayers.Where((w) => w.GameId.Equals(gameId));
        var skipCount = SkipCount(pagination);
        var currenPage = await winningplayers
            .Skip(skipCount)
            .Take(pagination.CurrentPageItems)
            .Include(wp => wp.User)
            .Select(wp => wp.User)
            .ToListAsync();
        var totalPages = await winningplayers.CountAsync();
        newPagination.CurrentPage = pagination.CurrentPage;
        newPagination.CurrentPageItems = pagination.CurrentPageItems;
        newPagination.NextPage = pagination.CurrentPage + 1;
        newPagination.HasNext = totalPages > 0;
        newPagination.TotalItems = totalPages;
        return new Tuple<List<User>, Pagination>(currenPage, newPagination);
    }


    private bool AllowedToBUy(int balance, int ticketPrice)
    {
        if (balance < ticketPrice)
        {
            return false;
        }

        return true;
    }


    private int ComputeBalance(int balance, int ticketPrice)
    {
        return balance - ticketPrice;
    }


    private GameTicket MapFromAutomatedTicket(AutomatedTicket automatedTicket, string gameId)
    {
        var gameTicket = new GameTicket
        {
            GUID = automatedTicket.Guid,
            PurchaseDate = automatedTicket.PurchaseDate,
            Sequence = automatedTicket.Sequence,
            UserId = automatedTicket.UserId,
            PriceValue = automatedTicket.PriceValue,
            GameId = gameId,
        };
        return gameTicket;
    }
}

public struct WeekYear<T1, T2> : IEquatable<WeekYear<T1, T2>>
{
    public T1 Week { get; }
    public T2 Year { get; }

    public WeekYear(T1 week, T2 year)
    {
        Week = week;
        Year = year;
    }

    public bool Equals(WeekYear<T1, T2> other)
    {
        return EqualityComparer<T1>.Default.Equals(Week, other.Week) &&
               EqualityComparer<T2>.Default.Equals(Year, other.Year);
    }

    public override bool Equals(object? obj)
    {
        if (obj is WeekYear<T1, T2> other)
        {
            return Equals(other);
        }

        return false;
    }


    public override int GetHashCode()
    {
        int hashWeek = Week != null ? Week.GetHashCode() : 0;
        int hashYear = Year != null ? Year.GetHashCode() : 0;
        return HashCode.Combine(hashWeek, hashYear);
    }

    public static bool operator ==(WeekYear<T1, T2> left, WeekYear<T1, T2> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WeekYear<T1, T2> left, WeekYear<T1, T2> right)
    {
        return !(left == right);
    }
}