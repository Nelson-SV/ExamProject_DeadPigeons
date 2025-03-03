using Common.SharedModels;

namespace Service.TransferModels.Responses.GameManagementDto;

public class ProcessedWinningSequence
{
    public bool Registered { get; set; }
    public string Message { get; set; } = "";
    public List<InsufficientFundDto> UninsertedTickets { get; set; } = new List<InsufficientFundDto>();
}