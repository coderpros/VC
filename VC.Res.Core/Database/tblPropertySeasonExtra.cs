using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertySeasonExtra")]
[Index("tblPropertyExtra_id", Name = "IX_tblPropertySeasonExtra_tblPropertyExtra_id")]
[Index("tblPropertySeason_id", Name = "IX_tblPropertySeasonExtra_tblPropertySeason_id")]
public partial class tblPropertySeasonExtra
{
    [Key]
    public int tblPropertySeasonExtra_id { get; set; }

    public int tblPropertySeason_id { get; set; }

    public int? tblPropertyExtra_id { get; set; }

    [StringLength(100)]
    public string tblPropertySeasonExtra_name { get; set; } = null!;

    public string? tblPropertySeasonExtra_desc { get; set; }

    public int tblPropertySeasonExtra_priceEntryMode { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertySeasonExtra_price { get; set; }

    public bool tblPropertySeasonExtra_commissionSubjectTo { get; set; }

    public int tblPropertySeasonExtra_commissionAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertySeasonExtra_commissionAmount { get; set; }

    public string? tblPropertySeasonExtra_commissionNote { get; set; }

    public bool tblPropertySeasonExtra_taxExempt { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertySeasonExtra_taxValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonExtra_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeasonExtra_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertySeasonExtra_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeasonExtra_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblPropertySeasonExtra_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertySeasonExtra_deletedBy { get; set; }

    [ForeignKey("tblPropertyExtra_id")]
    [InverseProperty("tblPropertySeasonExtras")]
    public virtual tblPropertyExtra? tblPropertyExtra { get; set; }

    [ForeignKey("tblPropertySeason_id")]
    [InverseProperty("tblPropertySeasonExtras")]
    public virtual tblPropertySeason tblPropertySeason { get; set; } = null!;
}
