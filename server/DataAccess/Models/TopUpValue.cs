using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class TopUpValue
{
    [Key]
    [Column("TopUpValue")]
    public int TopUpVal { get; set; }
}
