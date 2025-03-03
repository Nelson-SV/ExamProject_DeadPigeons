using DataAccess.Models;

namespace Service.TransferModels.Responses.GameManagementDto;

public class CurrentGameDto
{
    public string Guid { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public string FormattedStartDate => StartDate.ToString("yyyy/MM/dd");

    public DateTime? ExtractionDate { get; set; }

    public List<int>? ExtractedNumbers { get; set; }

    public int? Revenue { get; set; }

    public int? RolloverValue { get; set; }

    public bool Status { get; set; }


    public CurrentGameDto ConvertFromModelToDto(Game game)
    {
        return new CurrentGameDto
        {
            Guid = game.Guid,
            StartDate = game.StartDate,
            ExtractionDate = game.ExtractionDate,
             ExtractedNumbers = game.ExtractedNumbers==null? [] : game.ExtractedNumbers!.ToList(),
            Revenue = game.Revenue,
            RolloverValue = game.RolloverValue,
            Status = game.Status
        };
    }
}