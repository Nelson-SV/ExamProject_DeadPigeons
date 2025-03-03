using System.Net;
using System.Net.Http.Headers;
using API;
using DataAccess;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service;
using Service.Security;
using Xunit.Abstractions;
using CreateTicketDto = Service.TransferModels.Requests.CreateTicketDto;


namespace ApiInterationTests.CustomerTests;

public class TicketsTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public ITokenClaimsService tokenService;

    public TicketsTests(ApiTestBase factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
        tokenService = new JwtTokenClaimService();
    }

    private IServiceProvider ApplicationServices => _factory.Services;
    private HttpClient TestHttpClient { get; }

   [Fact]
    public async Task UserCannotBuyTickets_WhenNotEnoughBalance()
    {
        /*they need to be done for each test case so we avoid lifetime dependencies.*/
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var game = await ctx.Games.FirstOrDefaultAsync();
        Assert.NotNull(game); 
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (user != null)
        {
            var balance = new Balance
            {
                Guid = Guid.NewGuid(),
                UserId = user.Id,
                Value = 10 
            };

            ctx.Balances.Add(balance);
            await ctx.SaveChangesAsync();
        }
        var request = new CreateTicketDto
        {
            UserId = user.Id,
            GameId = game.Guid,
            Sequence = new[] { 1, 2, 3, 4, 5, 6, 7 },
            PriceValue = 80, 
            IsAutomated = false
        };

        /* Act & Assert: Attempt to create a ticket and expect an exception*/
        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await new PlayClient(TestHttpClient).CreateGameTicketAsync(new List<CreateTicketDto> { request })
        );

        /* Assert that the exception status code is BadRequest (400)*/
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);

        var errorMessage = exception.Message;
        Assert.Contains("Insufficient balance", errorMessage);

        /* Assert: Verify no tickets were created*/
        var ticketsCount = await ctx.GameTickets.CountAsync(t => t.UserId == user.Id && t.GameId == game.Guid);
        Assert.Equal(0, ticketsCount); 
        
    }


    [Fact]
    public async Task CreateTickets_CeatesTickets()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();


        ctx.Balances.RemoveRange(ctx.Balances);
        ctx.SaveChanges();

        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var game = await ctx.Games.FirstOrDefaultAsync();
        Assert.NotNull(game); 
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var balance = new Balance
        {
            Guid = Guid.NewGuid(),
            UserId = user.Id,
            Value = 900
        };

        ctx.Balances.Add(balance);
        await ctx.SaveChangesAsync();

        var request = new List<CreateTicketDto>
        {
            new()
            {
                UserId = user.Id, GameId = game.Guid, Sequence = new[] { 1, 2, 3, 4, 5, 6, 7 }, IsAutomated = true
            },
            new()
            {
                UserId = user.Id, GameId = game.Guid, Sequence = new[] { 8, 9, 10, 11, 12, 13, 14 }, IsAutomated = true
            }
        };

        var response = await new PlayClient(TestHttpClient).CreateGameTicketAsync(request);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);

    }

    [Fact]
    public async Task CreateTickets_Returns_ErrorWhen_WhenValidationWorks()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        
        var request = new List<CreateTicketDto>
        {
            new() { Sequence = new[] { 1, 2, 3, 4, 5, 6, 7 }, IsAutomated = true },
            new() { Sequence = new[] { 8, 9, 10, 11, 12, 13, 14 }, IsAutomated = false }
        };

        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await new PlayClient(TestHttpClient).CreateGameTicketAsync(request));

        Assert.NotNull(exception);

        /* Assert that the exception status code is BadRequest (400)*/
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);

        var errorMessage = exception.Message;

        Assert.Contains("The GameId field is required.", errorMessage);
        Assert.Contains("The UserId field is required.", errorMessage);

    }
    

    [Fact]
    public async Task CheckIsAllowedToPlay_ThrowsError_WhenGameBecomesInactive()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var game = await ctx.Games.FirstOrDefaultAsync();
        Assert.NotNull(game);

        game.Status = false;
        ctx.Games.Update(game);
        await ctx.SaveChangesAsync();


        var exception = await Assert.ThrowsAsync<ApiException>(async () =>
            await new PlayClient(TestHttpClient).CheckIsAllowedToPlayAsync());

        /* Assert that the exception status code is BadRequest (400)*/
        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
       

    }
    private void ClearDatabase(AppDbContext ctx)
    {
        ctx.Games.RemoveRange(ctx.Games);
        ctx.Users.RemoveRange(ctx.Users);
        ctx.SaveChanges();
    }
}