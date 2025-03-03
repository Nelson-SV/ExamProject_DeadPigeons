
namespace Service.TransferModels.Responses.BalanceDto;

public class CurrentBalanceDto
{
    public int BalanceValue { get; set; }
    
    public static CurrentBalanceDto FromEntity(DataAccess.Models.Balance balance)
    {
        return new CurrentBalanceDto()
        {
            BalanceValue = balance.Value
        };
    }
}