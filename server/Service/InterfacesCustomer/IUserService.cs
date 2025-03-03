using Service.TransferModels.Requests;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.Tickets;

namespace Service.InterfacesCustomer;

public interface IUserService
{
    GameTicketsPageResultDto GetUserTicketsHistory(string userId, int page, int pageSize);
}