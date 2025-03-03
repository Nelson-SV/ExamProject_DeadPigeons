namespace Service.TransferModels.Requests.Admin;

public class PaymentPageResultDto<PaymentDto>
{
    public List<PaymentDto> Items { get; set; }
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}