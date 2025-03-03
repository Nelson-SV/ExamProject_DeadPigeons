using DataAccess.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AutomatedTicket> AutomatedTickets { get; set; }
    public virtual DbSet<Balance> Balances { get; set; }
    public virtual DbSet<Game> Games { get; set; }
    public virtual DbSet<GameTicket> GameTickets { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<TicketPrice> TicketPrices { get; set; }
    public virtual DbSet<TopUpValue> TopUpValues { get; set; }
    public virtual DbSet<UserProfile> UserProfiles { get; set; }
    public virtual DbSet<WeeklyTicketSummary> WeeklyTicketSummaries { get; set; }
    public virtual DbSet<WinningPlayers> WinningPlayers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WinningPlayers>()
            .HasKey(wp => new { wp.GameId, wp.UserId });

        base.OnModelCreating(modelBuilder);
    }
}