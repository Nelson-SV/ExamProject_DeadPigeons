using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

[Table("UserProfile")]
public partial class UserProfile
{
    [Key]
    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool? isactive { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserProfile")]
    public virtual User User { get; set; } = null!;
}
