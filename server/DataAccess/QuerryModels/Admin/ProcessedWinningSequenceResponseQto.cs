using Common.SharedModels;
using DataAccess.Models;

namespace DataAccess.QuerryModels.Admin;

public class ProcessedWinningSequenceResponseQto
{
    public bool Registered { get; set; }
    public string Message { get; set; } = "";

    public List<InsufficientFundQto> UninsertedTickets { get; set; } = new List<InsufficientFundQto>();
}