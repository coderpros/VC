using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertySeasonDate")]
[Index("tblPropertySeason_id", Name = "IX_tblPropertySeasonDate_tblPropertySeason_id")]
public partial class tblPropertySeasonDate
{
    [Key]
    public int tblPropertySeasonDate_id { get; set; }

    public int tblPropertySeason_id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonDate_from { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonDate_to { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonDate_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeasonDate_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonDate_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeasonDate_editedBy { get; set; }

    [ForeignKey("tblPropertySeason_id")]
    [InverseProperty("tblPropertySeasonDates")]
    public virtual tblPropertySeason tblPropertySeason { get; set; } = null!;
}
