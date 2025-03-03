using Common.SharedModels;
using Service.TransferModels.Requests.Admin;
using Service.TransferModels.Requests.GameManagementRequestDto;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.GameManagementDto;


namespace Service.admin.GameManagement;

public interface IGameManagementService
{
    public Task<CurrentGameDto?> GetCurrentGame();
    public Task<bool> ValidateSequenceRequest(string gameId);
    Task<ProcessedWinningSequence> InsertWinningSequence(WinningSequenceDto winningSequence);
    Task<bool> ValidateGameId(GameIdDto gameId);
    Task<WinningPlayersDto> GetWinningPlayersForAGame(WinningPlayersRequestDto playersRequest);
    Task<PlayerTicketsResponseDto?> GetTicketsForPlayer(PlayerTicketsRequestDto playerTicketsRequest);
    Task SendUninsertedTicketsNotification(InsufficientFundDto insufficientFundDto);
    Task<List<CurrentGameDto>> FindGameByWeekAndYear(int gameRequestWeek, int gameRequestYear);
    Task<Tuple<List<CurrentGameDto>, Pagination>> GetGames(Pagination paginatio);

    Task<CurrentGameDto?> GetGameById(string guid);
}