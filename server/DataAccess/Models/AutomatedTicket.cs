using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Index("UserId", Name = "IX_AutomatedTickets_UserId")]
public partial class AutomatedTicket
{
    [Key]
    [Column("GUID")]
    public Guid Guid { get; set; }

    public DateTime PurchaseDate { get; set; }

    public int[] Sequence { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int PriceValue { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AutomatedTickets")]
    public virtual User User { get; set; } = null!;
}
