using System.Net;
using System.Net.Http.Headers;
using API;
using DataAccess;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Security;
using Xunit.Abstractions;

namespace ApiInterationTests.AdminTests;

public class PaymentTests: IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public PaymentTests(ApiTestBase factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }

    private IServiceProvider ApplicationServices => _factory.Services;
    private HttpClient TestHttpClient { get; }

  [Fact]
  public async Task UpdatePendingPayment_WhenAccepted()
{
    using var scope = _factory.Services.CreateScope();
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
    var user = await ctx.Users.Include(u => u.Roles).FirstOrDefaultAsync();
    var token = await tokenService.GetTokenAsync(user.Email!);
    TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    if (user == null)
    {
        throw new Exception("No user found for testing.");
    }

    var initialBalance = 10;
    var balance = new Balance
    {
        Guid = Guid.NewGuid(),
        UserId = user.Id,
        Value = initialBalance
    };
    ctx.Balances.Add(balance);

    var payment = new Payment
    {
        Guid = "5",
        Name = "Test Payment",
        Bucket = "TestBucket",
        TimeCreated = DateTime.UtcNow,
        MediaLink = "https://example.com/media/test",
        TransactionId = Guid.NewGuid().ToString(),
        Pending = true,
        Value = 200,
        UserId = user.Id
    };
    ctx.Payments.Add(payment);

    await ctx.SaveChangesAsync();
    
    var request = new UpdatePaymentDto
    {
        UserId = user.Id,
        Guid = payment.Guid,
        Value = payment.Value
    };

    var response = await new PaymentClient(TestHttpClient).UpdatePendingPaymentsAsync(request);

    Assert.Equal(200, (int)response.StatusCode);

    var updatedPayment = await ctx.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Guid == "5");

    Assert.NotNull(updatedPayment);
    Assert.False(updatedPayment.Pending);

    var updatedBalance = await ctx.Balances.AsNoTracking().FirstOrDefaultAsync(b => b.UserId == user.Id);

    Assert.NotNull(updatedBalance);
    Assert.Equal(initialBalance + payment.Value, updatedBalance.Value);
}
 

   
}