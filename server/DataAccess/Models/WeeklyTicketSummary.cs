using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("WeeklyTicketSummary")]
public partial class WeeklyTicketSummary
{
    [Key]
    public string GameId { get; set; } = null!;

    public int TotalTickets { get; set; }

    public int WinningTickets { get; set; }

    public int LosingTickets { get; set; }

    [ForeignKey("GameId")]
    [InverseProperty("WeeklyTicketSummary")]
    public virtual Game Game { get; set; } = null!;
}
