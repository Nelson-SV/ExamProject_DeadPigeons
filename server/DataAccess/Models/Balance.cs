using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("Balance")]
[Index("UserId", Name = "IX_Balance_UserId")]
public partial class Balance
{
    [Key]
    [Column("GUID")]
    public Guid Guid { get; set; }

    public int Value { get; set; }

    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Balances")]
    public virtual User User { get; set; } = null!;
}
