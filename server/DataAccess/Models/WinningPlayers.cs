using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public class WinningPlayers
{
    [Key]
    [Column(Order = 1)]
    [ForeignKey("User")]
    public string UserId { get; set; } = null!; // Foreign key to AspNetUsers table

    [Key]
    [Column(Order = 2)]
    [ForeignKey("Game")]
    public string GameId { get; set; } = null!; // Foreign key to Game table

    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Game Game { get; set; } = null!;
}