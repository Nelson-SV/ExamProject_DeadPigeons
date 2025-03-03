using Common.ErrorMessages;
using DataAccess.CustomerInterfaces;
using DataAccess.Models;
using Service.InterfacesCustomer;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.Tickets;

namespace Service;

public class UserService(
    IUserRepository userRepository
    ) : IUserService
{
    public GameTicketsPageResultDto GetUserTicketsHistory(string userId, int page, int pageSize)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException(ErrorMessages.GetMessage(ErrorCode.UserId));
        }

        var tickets = userRepository.GetUserGameTicketsHistory(userId, page, pageSize, out int totalTickets);

        if (tickets.Count == 0)
        {
            return new GameTicketsPageResultDto
            {
                Items = new List<GameTicketDetailedDto>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        var mappedTickets = tickets.Select(ticket =>
            {
                var detailedTicket = GameTicketDetailedDto.FromEntity(ticket);
                PopulateTicketDetails(detailedTicket);
                return detailedTicket;
            })
            .ToList();

        return new GameTicketsPageResultDto
        {
            Items = mappedTickets,
            TotalItems = totalTickets,
            Page = page,
            PageSize = pageSize
        };
    }

    private void PopulateTicketDetails(GameTicketDetailedDto ticket)
    {
        var game = userRepository.GetGameById(ticket.GameId);
        var weeklySummary = userRepository.GetSummaryByGameId(ticket.GameId);
        
        if (game.ExtractedNumbers == null || weeklySummary == null)
        {
            ticket.ExtractedNumbers = [];
            ticket.Winnings = 0;
            return;
        }
        
        ticket.ExtractedNumbers = game.ExtractedNumbers;
        ticket.Winnings = CalculateValueWon(game, weeklySummary, ticket.Sequence);

    }

    private decimal? CalculateValueWon(Game game, WeeklyTicketSummary summary, int[] sequence)
    {
        if (game.ExtractedNumbers == null || summary.WinningTickets <= 0 || game.ExtractedNumbers.Length == 0)
            return 0;
        
        var matchingNumbers = sequence.Intersect(game.ExtractedNumbers).Count();
        
        if (matchingNumbers < 3)
            return 0;

        var prizePool = game.Revenue * 0.7m; 
        var valueWon = prizePool / summary.WinningTickets;
        return valueWon == null ? 0 : Math.Round((decimal)valueWon, 2);
    }

}