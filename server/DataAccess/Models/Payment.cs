using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models;

public partial class Payment
{
    // The primary key column is GUID
    [Key]
    [Column("GUID")]
    public string Guid { get; set; } = null!;

    // 'name' column
    [Column("name")]
    public string Name { get; set; } = null!;

    // 'bucket' column
    [Column("bucket")]
    public string Bucket { get; set; } = null!;

    // 'timeCreated' column (timestamp with time zone)
    [Column("timeCreated")]
    public DateTime TimeCreated { get; set; }

    // 'updated' column (timestamp with time zone)
    [Column("updated")]
    public DateTime Updated { get; set; }

    // 'mediaLink' column
    [Column("mediaLink")]
    public string MediaLink { get; set; } = null!;

    // 'transactionId' column (varchar(250))
    [Column("transactionId")]
    public string? TransactionId { get; set; }

    // 'pending' column (boolean)
    [Column("pending")]
    public bool? Pending { get; set; }

    // 'value' column (integer)
    [Column("value")]
    public int Value { get; set; }

    // UserId column (foreign key to AspNetUsers)
    [Column("UserId")]
    public string UserId { get; set; } = null!;

    // Foreign key relationship with AspNetUser
    [ForeignKey("UserId")]
    [InverseProperty("Payments")]
    public virtual User User { get; set; } = null!;
}