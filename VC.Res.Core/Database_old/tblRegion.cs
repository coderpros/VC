using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblRegion")]
[Index("tblCountry_id", Name = "IX_tblRegion_tblCountry_id")]
public partial class tblRegion
{
    [Key]
    public int tblRegion_id { get; set; }

    public int tblCountry_id { get; set; }

    public int? tblRegion_websiteId { get; set; }

    [StringLength(100)]
    public string tblRegion_name { get; set; } = null!;

    [StringLength(200)]
    public string? tblRegion_desc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblRegion_createdUtc { get; set; }

    [StringLength(100)]
    public string? tblRegion_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblRegion_editedUtc { get; set; }

    [StringLength(100)]
    public string? tblRegion_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblRegion_deletedUtc { get; set; }

    [StringLength(100)]
    public string? tblRegion_deletedBy { get; set; }

    [ForeignKey("tblCountry_id")]
    [InverseProperty("tblRegions")]
    public virtual tblCountry tblCountry { get; set; } = null!;

    [InverseProperty("tblRegion")]
    public virtual ICollection<tblProperty> tblProperties { get; set; } = new List<tblProperty>();
}
