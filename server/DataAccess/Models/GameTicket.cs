using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Index("GameId", Name = "IX_GameTickets_GameId")]
[Index("UserId", Name = "IX_GameTickets_UserId")]
public partial class GameTicket
{
    [Key] [Column("Id")] public int Id { get; set; }
    public Guid GUID { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int[] Sequence { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int PriceValue { get; set; }

    public string GameId { get; set; } = null!;

    [ForeignKey("GameId")]
    [InverseProperty("GameTickets")]
    public virtual Game Game { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("GameTickets")]
    public virtual User User { get; set; } = null!;
}