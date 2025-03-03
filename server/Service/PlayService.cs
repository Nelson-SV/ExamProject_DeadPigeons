using Common.ErrorMessages;
using DataAccess.CustomerInterfaces;
using DataAccess.Models;
using Service.TransferModels.Requests;
using Service.TransferModels.Responses;
using Service.TransferModels.Responses.BalanceDto;


namespace Service;

public class PlayService(IPlayRepository playRepository) : IPlayService
{
    
     /*This method creates game tickets for a user and updates their balance.
     Key responsibilities:
     1. Ensure the user has sufficient balance.
     2. Generate unique identifiers (UUIDs) for the tickets.
     3. Separate automated and non-automated tickets for processing.
     4. Convert ticket DTOs into entities.
     5. Update the user's balance after deducting the ticket cost.
     6. Return the updated balance and the created automated tickets as DTOs for the response.*/

     public async Task <(CurrentBalanceDto, List<AutomatedTicketsDto>)> CreateGameTicket(List<CreateTicketDto> tickets)
    {
        var totalPrice = 0;
        var userId = tickets.First().UserId;
        var gameId = tickets.First().GameId;
        

        /* Load ticket prices once per request*/
        foreach (var ticket in tickets)
        {
            var price = ticket.PriceValue;
            ticket.PriceValue = price;
            totalPrice += price;
        }
        /* Generate UUIDs for all tickets*/
        var allTicketsMap = tickets.ToDictionary(
            dto => Guid.NewGuid(),
            dto => dto);
        
        var automatedTicketsMap = allTicketsMap.Where(pair => pair.Value.IsAutomated)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        /*  to entities */
        var gameTickets = CreateTicketDto.ToTickets(allTicketsMap);
        var automatedTickets = CreateTicketDto.ToAutomatedTickets(automatedTicketsMap);

        
        var balance = await CalculateBalance( totalPrice, userId);
        
        var (updatedBalance, createdAutomatedTickets) = await playRepository.ManageTickets(gameTickets, balance, userId, automatedTickets, gameId, totalPrice);
            var updatedBalanceDto = CurrentBalanceDto.FromEntity(updatedBalance);

            var automatedTicketsDtos = createdAutomatedTickets
                .Select(AutomatedTicketsDto.FromEntity)
                .ToList();
            
            return (updatedBalanceDto, automatedTicketsDtos);
            
    }

    private async Task <int> CalculateBalance(int totalPrice, string userId)
    {
        var balance = await playRepository.GetBalance(userId);
        
        if (balance < totalPrice)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.InsufficientBalance));
        }
        balance -= totalPrice;
        return balance;
        
    }
    

    public List<AutomatedTicketsDto> GetAutomatedTickets(string userId)
    {
        var automatedTickets = playRepository.GetAutomatedTickets(userId);
        return automatedTickets
            .Select(AutomatedTicketsDto.FromEntity)
            .ToList();
    }
    
    public async Task<GameIdDto> CheckIsAllowedToPlay()
    {
        var gameId = await playRepository.CheckIsAllowedToPlay();
        var gameIdDto = GameIdDto.FromGame(gameId);
        return gameIdDto;
       
    }

    public async Task<string> DeleteAutomatedTicket(AutomatedTicketsDto ticket)
    {
        var activeGame = await playRepository.GetActiveGame();
        if (activeGame == null)
        {
            await playRepository.DeleteAutomatedTicket(ticket.Guid);
            return "The ticket has been successfully removed.";
        }

        var automatedTicketEntity = AutomatedTicketsDto.ToEntity(ticket);
        bool isBought = await playRepository.IsAutomatedTicketBoughtForCurrentWeek(automatedTicketEntity, activeGame.Guid);

        await playRepository.DeleteAutomatedTicket(ticket.Guid);

        return isBought
            ? "The ticket was already purchased for the current week. But will no longer be purchased going forward."
            : "The ticket has been successfully removed.";
    }

    public async Task<string> UpdateAutomatedTicketStatus(AutomatedTicketsDto automatedTicket, bool isActive)
    {
        var activeGame = await playRepository.GetActiveGame();
        if (activeGame == null)
        {
            throw new ApplicationException(ErrorMessages.GetMessage(ErrorCode.NoActiveGames));
        }

        var automatedTicketEntity = AutomatedTicketsDto.ToEntity(automatedTicket);
        bool isBought = await playRepository.IsAutomatedTicketBoughtForCurrentWeek(automatedTicketEntity, activeGame.Guid);
        
        if (isBought)
        {
            await playRepository.UpdateAutomatedTicketStatus(automatedTicket.Guid, isActive, automatedTicketEntity);
            
            return isActive
                ? "The ticket will be activated going forward but was already bought for the current week."
                : "The ticket is still valid for this week's game but will no longer be purchased going forward.";
        }

        if (!isBought && isActive)
        {
            automatedTicketEntity.Guid = Guid.NewGuid();
            automatedTicketEntity.IsActive = isActive;
            automatedTicketEntity.PurchaseDate = DateTime.UtcNow;
            var automatedTicketList = new List<AutomatedTicket> { automatedTicketEntity };
            
            var ticket = AutomatedTicketsDto.ToGameTicketEntity(automatedTicket);
            ticket.GUID = automatedTicketEntity.Guid;
            ticket.GameId = activeGame.Guid;
            ticket.PurchaseDate = DateTime.UtcNow;
            var ticketList = new List<GameTicket>() {ticket};
            
            await playRepository.CreateGameTicket(ticketList);
            await playRepository.CreateAutomatedTicket(automatedTicketList);
            
            if (automatedTicketEntity.Guid != automatedTicket.Guid)
            {
                await playRepository.UpdateAutomatedTicketStatus(automatedTicket.Guid, true, automatedTicketEntity);
                return ("The ticket has been successfully activated and purchased for the current week.");
            }
        }
        await playRepository.UpdateAutomatedTicketStatus(automatedTicket.Guid, isActive, automatedTicketEntity);
        return ("The ticket will no longer be purchased going forward.");
    }



}
