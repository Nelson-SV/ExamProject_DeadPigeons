using API.Responses;
using Common.ErrorMessages;
using Common.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.admin.GameManagement;
using Service.TransferModels.Requests.GameManagementRequestDto;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.GameManagementDto;


namespace Api.Controllers.admin.GameHandler;

[ApiController]
[Route("api/admin")]
public class GameHandlerController : ControllerBase
{
    private ILogger<GameHandlerController> _logger;

    public GameHandlerController(ILogger<GameHandlerController> logger)
    {
        _logger = logger;
    }
/// <summary>
/// Retrieves the current game where the extracted numbers are null
/// </summary>
/// <param name="gameManagementService">The Service useed to process the logic</param>
/// <returns>A CurrentGameDto that represents the game data </returns>
    [HttpGet]
    [Route("currentGame")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<CurrentGameDto>> GetCurrentGamInfo(
        [FromServices] IGameManagementService gameManagementService)
    {
        var response = await gameManagementService.GetCurrentGame();
        if (response == null)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameRetrieveError));
        }

        return Ok(response);
    }

    private ActionResult CreateErrorMessage(string errorMessage)
    {
        ValidationErrors validation = new ValidationErrors
        {
            Message = new[] { errorMessage }
        };
        return BadRequest(new BadRequest(validation));
    }

    
    /// <summary>
    /// Insert into the database the winning sequence for the current game,and creates a new game.
    /// </summary>
    /// <param name="gameManagementService"></param>
    /// <param name="winningSequence">The extracted numbers for the current game</param>
    /// <returns></returns>
    [HttpPost]
    [Route("settWinningSequence")]
     [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<ProcessedWinningSequence>> SetWinningNumbers(
        [FromServices] IGameManagementService gameManagementService, [FromBody] WinningSequenceDto winningSequence)
    {
        var validateGame = await gameManagementService.ValidateSequenceRequest(winningSequence.GameId!);
        if (!validateGame)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameIdInvalid));
        }

        var response = await gameManagementService.InsertWinningSequence(winningSequence);
        if (!response.Registered)
        {
            return CreateErrorMessage(response.Message);
        }

        try
        {
            foreach (var user in response.UninsertedTickets)
            {
                await gameManagementService.SendUninsertedTicketsNotification(user);
            }
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Warning, e.Message, e);
        }

        return Ok(response);
    }

/// <summary>
/// Retrieves an object with a list of wining players for a game
/// </summary>
/// <param name="gameManagementService"></param>
/// <param name="playersRequest"> The request object, that will contain the gameId, winning numbers and pagination object</param>
/// <returns>An object that hold a list </returns>
    [HttpPost]
    [Route("getWinningPlayers")]
     [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<WinningPlayersDto>> GetWinningPlayers(
        [FromServices] IGameManagementService gameManagementService, [FromBody] WinningPlayersRequestDto playersRequest)
    {
        var validateGameId = await gameManagementService.ValidateGameId(playersRequest.GameId!);
        if (!validateGameId)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameIdInvalid));
        }

        var response = await gameManagementService.GetWinningPlayersForAGame(playersRequest);
        if (response.WinningPlayers == null)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.ErrorRetrievePlayers));
        }

        return Ok(response);
    }
  
  
    

/// <summary>
/// Retrieve information for a game by game id
/// </summary>
/// <param name="gameManagementService"></param>
/// <param name="game">Object that holds the gameId,from the request body</param>
/// <returns>An objet that represents the retrieved game data</returns>
    [HttpPost]
    [Route("getGameById")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<CurrentGameDto>> GetGameById(
        [FromServices] IGameManagementService gameManagementService, [FromBody] GameIdDto game)
    {
        var validateGameId = await gameManagementService.ValidateGameId(game);
        if (!validateGameId)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameIdInvalid));
        }

        try
        {
            var response = await gameManagementService.GetGameById(game.Guid);

            if (response == null)
            {
                return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameRetrieveError));
            }

            return Ok(response);
        }
        catch
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameRetrieveError));
        }
    }

/// <summary>
/// Retrieves a list with games, paginated used for the history
/// </summary>
/// <param name="gameManagementService"></param>
/// <param name="pagination"></param>
/// <returns></returns>
    [HttpPost]
    [Route("getGames")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<Tuple<List<CurrentGameDto>,Pagination>>> GetGames(
        [FromServices] IGameManagementService gameManagementService, [FromBody] Pagination pagination)
    {
        try
        {
            var response = await gameManagementService.GetGames(pagination);
            return Ok(response);
        }
        catch
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.GameRetrieveError));
        }
    }
    
    
/// <summary>
/// Retrieve the winning tickets for a player
/// </summary>
/// <param name="gameManagementService"></param>
/// <param name="playerTicketsRequest">A list of player tickets id</param>
/// <returns>Retrieve the winning tickets for a player</returns>
    [HttpPost]
    [Route("getWinningTicketsForPlayer")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<PlayerTicketsResponseDto>> GetWinningTicketsForPlayer(
        [FromServices] IGameManagementService gameManagementService,
        [FromBody] PlayerTicketsRequestDto playerTicketsRequest)
    {
        var response = await gameManagementService.GetTicketsForPlayer(playerTicketsRequest);
        if (response == null)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.ErrorRetrieveTickets));
        }
        return Ok(response);
    }

 /// <summary>
 /// Retrieve a game by week and year, used in the search request
 /// </summary>
 /// <param name="gameRequest"></param>
 /// <param name="gameManagementService">Week and year to search for</param>
 /// <returns>List of games that satisfy the filter</returns>

    [HttpPost]
    [Route("getGameByWeekAndYear")]
    [Authorize(Roles = Role.Admin)]
    public async Task<ActionResult<List<CurrentGameDto>>> GetGameByWeekAndYear(
        [FromBody] GetGameRequestDto gameRequest,
        [FromServices] IGameManagementService gameManagementService)
    {
        try
        {
            var result = await gameManagementService.FindGameByWeekAndYear(gameRequest.Week, gameRequest.Year);
            return Ok(result);
        }
        catch (Exception e)
        {
            return CreateErrorMessage(ErrorMessages.GetMessage(ErrorCode.UnexpectedError));
        }
    }
    
}