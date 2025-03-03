using Service.TransferModels.Responses.Tickets;
using Service.TransferModels.Responses.UserDetails;

namespace Service.TransferModels.Responses;

public interface IPageResultDto
{
    int TotalItems { get; set; }
    int Page { get; set; }
    int PageSize { get; set; }
}

public class GameTicketsPageResultDto : IPageResultDto
{
    public required List<GameTicketDetailedDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UsersDetailsPageResultDto : IPageResultDto
{
    public List<UsersDetailsDto>? Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}