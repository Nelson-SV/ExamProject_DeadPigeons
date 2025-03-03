using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("Game")]
public partial class Game
{
    [Key]
    [Column("GUID")]
    public string Guid { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? ExtractionDate { get; set; }

    public int[]? ExtractedNumbers { get; set; }

    public int? Revenue { get; set; }

    public int? RolloverValue { get; set; }
    
    public bool Status { get; set; }

    [InverseProperty("Game")]
    public virtual ICollection<GameTicket> GameTickets { get; set; } = new List<GameTicket>();

    [InverseProperty("Game")]
    public virtual WeeklyTicketSummary? WeeklyTicketSummary { get; set; }
}
