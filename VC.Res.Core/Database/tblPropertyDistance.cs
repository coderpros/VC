using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyDistance")]
[Index("tblProperty_id", Name = "IX_tblPropertyDistance_tblProperty_id")]
public partial class tblPropertyDistance
{
    [Key]
    public int tblPropertyDistance_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblPropertyDistance_type { get; set; }

    [StringLength(100)]
    public string? tblPropertyDistance_name { get; set; }

    public string? tblPropertyDistance_desc { get; set; }

    public double tblPropertyDistance_distanceKM { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? tblPropertyDistance_lat { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? tblPropertyDistance_long { get; set; }

    public int? tblPropertyDistance_minByWalk { get; set; }

    public int? tblPropertyDistance_minByDrive { get; set; }

    public int? tblPropertyDistance_minByBoat { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyDistance_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyDistance_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyDistance_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyDistance_editedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyDistances")]
    public virtual tblProperty tblProperty { get; set; } = null!;
}
