using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Index("NumberOfFields", "Price", Name = "UQ_NumberOfFields_Price", IsUnique = true)]
public partial class TicketPrice
{
    [Key]
    [Column("GUID")]
    public Guid Guid { get; set; }
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

    public int NumberOfFields { get; set; }

    public int Price { get; set; }
}
