using Bogus;
using DataAccess.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Service;
using Service.TransferModels.Requests;

namespace ApiInterationTests;

public class TestObjects
{
    public static User GetUser()
    {
        var passwordHasher = new PasswordHasher<User>();
        return new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpper())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpper())
            .RuleFor(u => u.EmailConfirmed, f => true)
            .RuleFor(u => u.SecurityStamp, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.ConcurrencyStamp, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.PasswordHash, (f, u) => passwordHasher.HashPassword(u, "BogusPassword1!"));
        
    }

   

    public static Game GetGame()
    {
        return new Faker<Game>()
            .RuleFor(g => g.Guid, f => Guid.NewGuid().ToString())
            .RuleFor(g => g.StartDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(g => g.ExtractionDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(g => g.Status, true)
            .RuleFor(g => g.ExtractedNumbers, f => f.Make(3, () => f.Random.Int(1, 16)).ToArray());
    }
    
    public static Game GetGameInProgress()
    {
        return new Faker<Game>()
            .RuleFor(g => g.Guid, f => Guid.NewGuid().ToString())
            .RuleFor(g => g.StartDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(g => g.ExtractionDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(g => g.Status, true)
            .RuleFor(g => g.ExtractedNumbers, f =>null);
    }


    public static GameTicket CreateGameTicket(int ticketId,string gameId,string userId,int[] playedNumbers)
    {
        return new Faker<GameTicket>()
            .RuleFor(gt => gt.Id, f => ticketId)
            .RuleFor(gt => gt.PurchaseDate, f => f.Date.Past().ToUniversalTime())
            .RuleFor(gt => gt.GUID, f => Guid.NewGuid())
            .RuleFor(gt => gt.GameId, gameId)
            .RuleFor(gt => gt.UserId, userId)
            .RuleFor(gt => gt.Sequence, playedNumbers)
            .RuleFor(gt => gt.PriceValue, f => f.Random.Int());
    }
    
    


}