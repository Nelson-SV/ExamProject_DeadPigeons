using API.Extensions;
using api.Middleware;
using Common;
using Common.EmailTemplates;
using Common.SharedModels;
using DataAccess;
using DataAccess.admin.GameManagementRepository;
using DataAccess.BalanceRepository;
using DataAccess.ConfigurationsRepository;
using DataAccess.CustomerInterfaces;
using DataAccess.CustomerRepositories;
using DataAccess.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using Quartz;
using Serilog;
using Service;
using Service.admin.GameManagement;
using Service.AdminService;
using Service.AdminService.Payment;
using Service.Balance;
using Service.ImagePersistance;
using Service.InterfacesCustomer;
using Service.Security;
using Service.TransferModels.Responses.Configurations;
using Service.Validators;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        /* Configure Serilog*/
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        /* Replace default logging with Serilog*/
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();


        /* Bind EmailSettings*/
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("AppOptions"));

        builder.Services.AddTransient<EmailService>();


        /*Scheduler*/
        builder.Services.AddQuartz(configure =>
        {
            /* Configure the job and trigger*/
            var jobKey = new JobKey("DatabaseUpdateJob");

            /* Register the job and add a trigger*/
            configure.AddJob<DatabaseUpdateJob>(opts => opts.WithIdentity(jobKey));

            /* Create a trigger for the job (every Saturday at 5:00 PM Danish time)*/
            configure.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("DatabaseUpdateTrigger")
                .StartNow()
                .WithCronSchedule("0 0 17 ? * 6"));
        });

        /* Add Quartz hosted service*/
        builder.Services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });


        builder.Services.AddScoped<IDatabaseUpdateService, DatabaseUpdateService>();
        builder.Services.AddScoped<IDataBaseUpdateRepository, DatabaseUpdateRepository>();
        
        // Fetch SendGrid API Key from Environment Variables
        var sendGridApiKey = Environment.GetEnvironmentVariable("sendgrid");

        if (!string.IsNullOrEmpty(sendGridApiKey))
        {
            builder.Services.Configure<AppOptions>(options =>
            {
                options.SendGridApiKey = sendGridApiKey; // Set the API key from environment variables
                options.SendGridApiKey = sendGridApiKey; // Set the API key from environment variables
            });
        }
        else
        {
            // Fallback to reading from appsettings if the API key is not in environment variables
            builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("AppOptions"));
        }


        #region Configuration

        builder.Services.AddSingleton(_ => TimeProvider.System);

        builder.AdddAppOptions();
        builder.AddPgContainer();
        builder.Services.Configure<AppOptions>(builder.Configuration.GetSection(nameof(AppOptions)));
        builder.Services.AddOptions<AppOptions>()
            .Bind(builder.Configuration.GetSection(nameof(AppOptions)));
        builder.Configuration.AddEnvironmentVariables();
        var options = builder.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>()!;
        builder.Configuration.AddEnvironmentVariables();
        builder.Services.AddTransient<AppOptions>();
        builder.Services.AddTransient<AppOptions>();
        builder.Services.AddScoped<IImagePersistanceService, GoogleCloudPersistance>();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        builder.Services.AddScoped<Argon2idPasswordHasher<User>>();
        builder.Services.AddScoped<IPlayService, PlayService>();
        builder.Services.AddScoped<IPlayRepository, PlayRepository>();
        builder.Services.AddScoped<IBalanceRepository, BalanceRepository>();
        builder.Services.AddScoped<ConfigurationRepository>();
        builder.Services.AddScoped<ConfigurationService>();
        builder.Services.AddScoped<IUpdateBalance, UpdateBalanceService>();
        builder.Services.AddScoped<IImagePersistanceService, GoogleCloudPersistance>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddTransient<Pagination>();
        builder.Services.AddScoped<IGameManagementService, GameManagementService>();
        builder.Services.AddScoped<IGameManagementRepository, GameManagementRepository>();
        builder.Services.AddScoped<IAdminUserManagementRepository, AdminUserManagementRepository>();
        builder.Services.AddScoped<TemplateReader>();
        builder.Services.AddScoped<IAdminUserManagementService, AdminUserManagementService>();

        #endregion


        #region DataAccess

        var connectionString = builder.Configuration.GetConnectionString("AppDb");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(Environment.GetEnvironmentVariable("DB") ?? connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
        );

        #endregion


        #region Security

        builder.Services.AddScoped<ITokenClaimsService, JwtTokenClaimService>();
        builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddSingleton<IPasswordHasher<User>, Argon2idPasswordHasher<User>>();

        builder.Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o => { o.TokenValidationParameters = JwtTokenClaimService.ValidationParameters(options); });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .Build();
        });


        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        #endregion

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer(); // Add this line

        builder.Services.AddOpenApiDocument(configuration =>
        {
            {
                configuration.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Scheme = "Bearer ",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });
                //configuration.AddTypeToSwagger<T>(); //If you need to add some type T to the Swagger known types
                configuration.DocumentProcessors.Add(new MakeAllPropertiesRequiredProcessor());

                configuration.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            }
        });
        builder.Services.AddScoped<GoogleCloudPersistance>();

        
         builder.Services.AddCors(options =>
         {
             options.AddPolicy("AllowSpecificOrigin", builder =>
             {
                 builder.WithOrigins("https://deadpigeon-acfe6.web.app") // Allow your frontend origin
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials(); // If you're using credentials like cookies
             });
         });      

        var app = builder.Build();

        #region Initialize Roles and User

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            // Get services
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var ctx = services.GetRequiredService<AppDbContext>();
            var passwordTest = "BogusPassword1!";
            var passwordPlayerTest = "Player1!";
            var playerEmail = "player@yahoo.com";
            var roleAdmin = "Admin";
            var rolePlayer = "Player";

            // Ensure roles exist
            if (!roleManager.RoleExistsAsync(roleAdmin).Result)
            {
                roleManager.CreateAsync(new IdentityRole(roleAdmin)).Wait();
            }

            if (!roleManager.RoleExistsAsync(rolePlayer).Result)
            {
                roleManager.CreateAsync(new IdentityRole(rolePlayer)).Wait();
            }

            // Check if user exists
            var existingUser = userManager.FindByEmailAsync("InitialUser@yahoo.com").Result;
            var existingPlayer= userManager.FindByEmailAsync(playerEmail).Result;
            if (existingUser == null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "InitialUser",
                    NormalizedUserName = "INITIALUSER",
                    Email = "InitialUser@yahoo.com",
                    EmailConfirmed = true,
                };

                try
                {
                    // Create user
                    var result = userManager.CreateAsync(user, passwordTest).Result;
                    if (result.Succeeded)
                    {
                        // Add user to role
                        userManager.AddToRoleAsync(user, roleAdmin).Wait();
                        Console.WriteLine("User created and assigned Admin role successfully.");
                    }
                    else
                    {
                        // Log errors if creation failed
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
            
            if (existingPlayer == null)
            {
                var user = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "PlayerUser",
                    NormalizedUserName = "PLAYERUSER",
                    Email = playerEmail,
                    EmailConfirmed = true,
                };

                try
                {
                    // Create user
                    var result = userManager.CreateAsync(user,passwordPlayerTest).Result;
                    if (result.Succeeded)
                    {
                        // Add user to role
                        userManager.AddToRoleAsync(user, rolePlayer).Wait();
                        Console.WriteLine("User created and assigned Admin role successfully.");
                    }
                    else
                    {
                        // Log errors if creation failed
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("User already exists.");
            }
            
            
            
        }

        #endregion

      

        app.MapGet("/", (ILogger<Program> logger) =>
        {
            logger.LogInformation("This is logged to both console and file!");
            return "Check your logs!";
        });

        app.UseHttpsRedirection();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        app.UseRouting();
        

        //app.MapIdentityApi<IdentityUser>().AllowAnonymous();


        app.UseOpenApi();
        app.UseSwaggerUi();
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                context.Request.Headers["Authorization"] = string.Empty;
            }

            await next();
        });
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        app.UseMiddleware<Middleware>();
        app.UseStatusCodePages();


        using (var scope = app.Services.CreateScope())
        {
            //   var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // context.Database.EnsureDeleted();
            //  context.Database.EnsureCreated();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!roleManager.RoleExistsAsync(Role.Player).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(Role.Player)).GetAwaiter().GetResult();
            }

            if (!roleManager.RoleExistsAsync(Role.Admin).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(Role.Admin)).GetAwaiter().GetResult();
            }
            //File.WriteAllText("current_db.sql", context.Database.GenerateCreateScript());
        }

        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        var url = $"http://0.0.0.0:{port}";

        app.Run(url);
    }
}
