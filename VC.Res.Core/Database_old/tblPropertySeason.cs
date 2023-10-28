using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertySeason")]
[Index("tblProperty_id", Name = "IX_tblPropertySeason_tblProperty_id")]
public partial class tblPropertySeason
{
    [Key]
    public int tblPropertySeason_id { get; set; }

    public int tblProperty_id { get; set; }

    [StringLength(100)]
    public string? tblPropertySeason_name { get; set; }

    public string? tblPropertySeason_noteInt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeason_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeason_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeason_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeason_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblPropertySeason_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeason_deletedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertySeasons")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [InverseProperty("tblPropertySeason")]
    public virtual ICollection<tblPropertyConfig> tblPropertyConfigs { get; set; } = new List<tblPropertyConfig>();

    [InverseProperty("tblPropertySeason")]
    public virtual ICollection<tblPropertyRate> tblPropertyRates { get; set; } = new List<tblPropertyRate>();

    [InverseProperty("tblPropertySeason")]
    public virtual ICollection<tblPropertySeasonDate> tblPropertySeasonDates { get; set; } = new List<tblPropertySeasonDate>();

    [InverseProperty("tblPropertySeason")]
    public virtual ICollection<tblPropertySeasonExtra> tblPropertySeasonExtras { get; set; } = new List<tblPropertySeasonExtra>();
}
