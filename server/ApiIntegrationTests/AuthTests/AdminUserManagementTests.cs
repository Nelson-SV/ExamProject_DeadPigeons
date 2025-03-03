using System.Net;
using System.Net.Http.Headers;
using API;
using DataAccess;
using DataAccess.Models;
using Generated;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.Security;
using Xunit.Abstractions;

namespace ApiInterationTests.AuthTests;

public class AdminUserManagementTests : IClassFixture<ApiTestBase>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    private IServiceProvider ApplicationServices => _factory.Services;
    private HttpClient TestHttpClient { get; }
    private string TEST_PASS = "adminA1!";
    
    public AdminUserManagementTests(ApiTestBase factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _factory = factory;
        TestHttpClient = _factory.CreateClient();
    }
    
    [Fact]
    public async Task Register_Success_ReturnsOk()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var user = TestObjects.GetUser();
        await userManager.CreateAsync(user, TEST_PASS);
        await userManager.AddToRoleAsync(user, "Admin");

        var registerRequest = new RegisterRequest()
        {
            Email = "test@email.com",
            PhoneNumber = "12341234",
            Name = "Test",
            Role = "Player"
        };
        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var resultRequest = await new AuthClient(TestHttpClient).RegisterAsync(registerRequest);
        
        Assert.Equal((int)HttpStatusCode.OK, resultRequest.StatusCode);
        ClearDatabase(ctx);
    }
    
    [Fact]
    public async Task Register_UserCreationFails_ReturnsBadRequest()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var user = TestObjects.GetUser();
        await userManager.CreateAsync(user, TEST_PASS);
        await userManager.AddToRoleAsync(user, "Admin");
        
        var registerRequest = new RegisterRequest { Role = "Player", Name = "Test", Email = "test@test.com", PhoneNumber = "3456789012" };

        var token = await tokenService.GetTokenAsync(user.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        try
        {
            var resultRequest = await new AuthClient(TestHttpClient).RegisterAsync(registerRequest);
            Assert.Fail("Expected a BadRequest response, but the request succeeded.");
        }
        catch (ApiException ex)
        {
            _testOutputHelper.WriteLine($"API Exception: {ex.Message}");
            Assert.Equal(400, ex.StatusCode);
        }
        ClearDatabase(ctx);
    }
    
    [Fact]
    public async Task GetUsersDetails_ShouldReturnOk_WhenValidParametersArePassed()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        
        var admin = TestObjects.GetUser();
        var player1Test = TestObjects.GetUser();
        var player2Test = TestObjects.GetUser();

        await userManager.CreateAsync(admin, TEST_PASS);
        await userManager.CreateAsync(player1Test, TEST_PASS);
        await userManager.CreateAsync(player2Test, TEST_PASS);
        
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.AddToRoleAsync(player1Test, "Player");
        await userManager.AddToRoleAsync(player2Test, "Player");

        var token = await tokenService.GetTokenAsync(admin.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var player1 = new UserProfile()
        {
            UserId = player1Test.Id,
            UserName = player1Test.UserName,
            isactive = true
        };

        var player2 = new UserProfile()
        {
            UserId = player2Test.Id,
            UserName = player2Test.UserName,
            isactive = false
        };
        var players = new List<UserProfile> { player1, player2 };

        ctx.UserProfiles.AddRange(players);
        await ctx.SaveChangesAsync();

        var response = await new AdminUserManagementClient(TestHttpClient)
            .GetUsersDetailsAsync(admin.Id, page: 1, pageSize: 7);

        Assert.Equal((int)HttpStatusCode.OK, response.StatusCode);
        ClearDatabase(ctx);
    }
    
    [Fact]
    public async Task UpdateUser_Success_ReturnsUpdatedUserDetails()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
    
        var admin = TestObjects.GetUser();
        var player = TestObjects.GetUser();
        await userManager.CreateAsync(admin, TEST_PASS);
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.CreateAsync(player, TEST_PASS);
        await userManager.AddToRoleAsync(player, "Player");
    
        var token = await tokenService.GetTokenAsync(admin.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var player1 = new UserProfile()
        {
            UserId = player.Id,
            UserName = player.UserName,
            isactive = true
        };
        ctx.UserProfiles.Add(player1);
        await ctx.SaveChangesAsync();
        
        var updateUserDto = new UpdateUserDto
        {
            UserId = player.Id,
            Name = "Updated",
            Email = "updated@mail.com",
            PhoneNumber = "00099966",
            IsActive = true
        };
    
        var result = await new AuthClient(TestHttpClient).UpdateUserAsync(updateUserDto);
    
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Result.Name);
        Assert.Equal("updated@mail.com", result.Result.Email);
        Assert.Equal("00099966", result.Result.PhoneNumber);
    
        //ClearDatabase(ctx);
    }

    
    [Fact]
    public async Task DeleteUser_ShouldReturnOk_WhenUserIsDeleted()
    {
        using var scope = _factory.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenClaimsService>();
    
        var admin = TestObjects.GetUser();
        var player = TestObjects.GetUser();
        await userManager.CreateAsync(admin, TEST_PASS);
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.CreateAsync(player, TEST_PASS);
        await userManager.AddToRoleAsync(player, "Player");
    
        var token = await tokenService.GetTokenAsync(admin.Email!);
        TestHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
        var result = await new AdminUserManagementClient(TestHttpClient).DeleteUserAsync(player.Id);
    
        Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
    }

 
    private void ClearDatabase(AppDbContext ctx)
    {
        ctx.Users.RemoveRange(ctx.Users);
        ctx.UserProfiles.RemoveRange(ctx.UserProfiles);
        ctx.SaveChanges();
    }
    
}