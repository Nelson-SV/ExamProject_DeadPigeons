using System.Text;
using Common.SharedModels;
using DataAccess.admin.GameManagementRepository;
using DataAccess.Models;
using DataAccess.QuerryModels.Admin;
using Service.AdminService;
using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Requests.GameManagementRequestDto;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.GameManagementDto;

namespace Service.admin.GameManagement;

public class GameManagementService : IGameManagementService
{
    private IGameManagementRepository _gameManagementRepository;
    private EmailService _emailService;


    public GameManagementService(IGameManagementRepository gameManagementRepository, EmailService emailService)
    {
        _gameManagementRepository = gameManagementRepository;
        _emailService = emailService;
    }

    public async Task<CurrentGameDto?> GetCurrentGame()
    {
        var response = await _gameManagementRepository.GetCurrentGame();
        if (response != null)
        {
            return new CurrentGameDto().ConvertFromModelToDto(response);
        }

        return null;
    }

    public async Task<bool> ValidateSequenceRequest(string gameId)
    {
        return await _gameManagementRepository.ValidateSequenceRequest(gameId);
    }

    public async Task<ProcessedWinningSequence> InsertWinningSequence(WinningSequenceDto winningSequence)
    {
        var responseTickets = new ProcessedWinningSequence();
        var response = await _gameManagementRepository.ProcessWinningSequence(winningSequence.WinningSequence!,
            winningSequence.GameId!);

        foreach (var ticketResponse in response.UninsertedTickets)
        {
            responseTickets.UninsertedTickets.Add(mapRepositoryToService(ticketResponse));
        }

        responseTickets.Registered = response.Registered;
        responseTickets.Message = response.Message;

        return responseTickets;
    }

    public async Task<bool> ValidateGameId(GameIdDto gameId)
    {
        var response = await _gameManagementRepository.ValidateGameAsync(gameId.Guid);
        return response;
    }

    public async Task<WinningPlayersDto> GetWinningPlayersForAGame(WinningPlayersRequestDto winningPlayers)
    {
        var responseTickets = new WinningPlayersDto();
        var response = await _gameManagementRepository.GetWinningPlayers(winningPlayers.GameId!.Guid,
            winningPlayers.Pagination!, winningPlayers.WinningSequence!);
        if (response.WinningTickets == null)
        {
            return responseTickets;
        }

        responseTickets.WinningPlayers = ProcessWinners(response.WinningTickets!);
        responseTickets.Pagination = response.Pagination;

        return responseTickets;
    }

    public async Task<PlayerTicketsResponseDto?> GetTicketsForPlayer(PlayerTicketsRequestDto playerTicketsRequest)
    {
        var response = await _gameManagementRepository.GetTicketsForPlayer(playerTicketsRequest!.PlayerTicketsIds!)!;
        if (response != null)
        {
            var playerTicketsResponse = new PlayerTicketsResponseDto();
            playerTicketsResponse.PlayerTickets = new List<TicketsResponseDto>();
            foreach (var ticket in response)
            {
                playerTicketsResponse.PlayerTickets.Add(TicketsResponseDto.FromEntity(ticket));
            }

            return playerTicketsResponse;
        }

        return null;
    }


    /// <summary>
    /// Processes a list of game tickets and aggregates the winning details for each unique player.
    /// </summary>
    /// <param name="gameTickets">A list of <see cref="GameTicket"/> objects representing tickets in the game.</param>
    /// <returns>
    /// A list of <see cref="WinningPlayer"/> objects, each representing a unique player with their aggregated winning details.
    /// </returns>
    /// <remarks>
    /// This method groups game tickets by the player's ID and calculates the total number of wins for each player.
    /// It also collects the GUIDs of all winning tickets associated with each player.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// var gameTickets = new List<GameTicket>
    /// {
    ///     new GameTicket { UserId = "1", Guid = Guid.NewGuid(), User = new User { UserName = "Alice", Email = "alice@example.com" } },
    ///     new GameTicket { UserId = "1", Guid = Guid.NewGuid(), User = new User { UserName = "Alice", Email = "alice@example.com" } },
    ///     new GameTicket { UserId = "2", Guid = Guid.NewGuid(), User = new User { UserName = "Bob", Email = "bob@example.com" } }
    /// };
    /// var winners = ProcessWinners(gameTickets);
    /// </code>
    /// </example>
    private List<WinningPlayer> ProcessWinners(List<GameTicket> gameTickets)
    {
        var winnersTickets = new Dictionary<string, WinningPlayer>();
        foreach (var ticket in gameTickets)
        {
            if (winnersTickets.TryGetValue(ticket.UserId, out var winnersTicket))
            {
                winnersTicket.WinningCount++;
                winnersTicket.WinningTicketsIds.Add(ticket.GUID);
            }
            else
            {
                winnersTickets[ticket.UserId] = new WinningPlayer
                {
                    PlayerId = ticket.UserId,
                    UserName = ticket.User.UserName,
                    Email = ticket.User.Email,
                    WinningTicketsIds = [ticket.GUID],
                    WinningCount = 1
                };
            }
        }

        return winnersTickets.Values.ToList();
    }

    private InsufficientFundDto mapRepositoryToService(InsufficientFundQto insufficientFundQto)
    {
        var insuficientFundDto = new InsufficientFundDto
        {
            UserName = insufficientFundQto.UserName,
            UserEmail = insufficientFundQto.UserEmail,
            BalanceValue = insufficientFundQto.BalanceValue,
            CurrentGameDate = insufficientFundQto.CurrentGameDate,
        };
        foreach (var ticket in insufficientFundQto.UninsertedTickets)
        {
            insuficientFundDto.FailedToInsertTickets.Add(GameTicketDto.FromEntity(ticket));
        }

        return insuficientFundDto;
    }

    public async Task SendUninsertedTicketsNotification(InsufficientFundDto insufficientFundDto)
    {
        var userEmail = insufficientFundDto.UserEmail;
        var template = _emailService.LoadEmailTemplate("UninsertedTicketsNotification.html");
        if (string.IsNullOrEmpty(template))
        {
            throw new InvalidOperationException("Email template could not be loaded.");
        }

        var placeholders = new Dictionary<string, string>
        {
            { "{{userName}}", insufficientFundDto.UserName },
            { "{{userEmail}}", userEmail },
            { "{{balanceValue}}", insufficientFundDto.BalanceValue.ToString() },
            { "{{currentGameDate}}", insufficientFundDto.CurrentGameDate.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        template = ReplacePlaceholders(template, placeholders);


        if (insufficientFundDto.FailedToInsertTickets == null || !insufficientFundDto.FailedToInsertTickets.Any())
        {
            throw new InvalidOperationException("No failed tickets to include in the notification.");
        }

        var ticketRows = new StringBuilder();
        foreach (var ticket in insufficientFundDto.FailedToInsertTickets)
        {
            ticketRows.Append($@"
        <tr>
            <td>{ticket.PurchaseDate.ToString("yyyy-MM-dd HH:mm:ss")}</td>
            <td>{string.Join(", ", ticket.Sequence)}</td>
            <td>{ticket.PriceValue}</td>
        </tr>");
        }


        template = template.Replace("{{#each failedToInsertTickets}}", ticketRows.ToString())
            .Replace("{{/each}}", "");

        await _emailService.SendEmailAsync(userEmail, "Balance insufficient", template);
    }

    public async Task<List<CurrentGameDto>> FindGameByWeekAndYear(int gameRequestWeek, int gameRequestYear)
    {
        var gameDtoList = new List<CurrentGameDto>();
        var result = await _gameManagementRepository.FindGameByWeekAndYear(gameRequestWeek, gameRequestYear);
        foreach (var game in result)
        {
            var currentGame = new CurrentGameDto().ConvertFromModelToDto(game);
            gameDtoList.Add(currentGame);
        }

        return gameDtoList;
    }

    /// <summary>
    /// Retrieve the games list and the pagination object after the page processing
    /// </summary>
    public async Task<Tuple<List<CurrentGameDto>, Pagination>> GetGames(Pagination pagination)
    {
        var gamesResponse = new List<CurrentGameDto>();
        var response = await _gameManagementRepository.GetGames(pagination);

        foreach (var resp in response.Item1)
        {
            gamesResponse.Add(new CurrentGameDto().ConvertFromModelToDto(resp));
        }

        return new Tuple<List<CurrentGameDto>, Pagination>(gamesResponse, response.Item2);
    }

    public async Task<CurrentGameDto?> GetGameById(string guid)
    {
        var response = await _gameManagementRepository.GetGameById(guid);
        return response != null ? new CurrentGameDto().ConvertFromModelToDto(response) : null;
    }


    public static string ReplacePlaceholders(string template, Dictionary<string, string> placeholders)
    {
        foreach (var placeholder in placeholders)
        {
            if (template.Contains(placeholder.Key))
            {
                template = template.Replace(placeholder.Key, placeholder.Value);
            }
            else
            {
                Console.WriteLine($"Warning: Placeholder {placeholder.Key} not found in the template.");
            }
        }

        return template;
    }
}