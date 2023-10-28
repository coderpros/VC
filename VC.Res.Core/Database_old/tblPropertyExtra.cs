using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyExtra")]
[Index("tblProperty_id", Name = "IX_tblPropertyExtra_tblProperty_id")]
public partial class tblPropertyExtra
{
    [Key]
    public int tblPropertyExtra_id { get; set; }

    public int tblProperty_id { get; set; }

    [StringLength(100)]
    public string tblPropertyExtra_name { get; set; } = null!;

    public string? tblPropertyExtra_desc { get; set; }

    public bool tblPropertyExtra_priceEntryModeInh { get; set; }

    public int tblPropertyExtra_priceEntryMode { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertyExtra_price { get; set; }

    public bool tblPropertyExtra_commissionSubjectTo { get; set; }

    public bool tblPropertyExtra_commissionInh { get; set; }

    public int tblPropertyExtra_commissionAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyExtra_commissionAmount { get; set; }

    public string? tblPropertyExtra_commissionNote { get; set; }

    public bool tblPropertyExtra_taxInh { get; set; }

    public bool tblPropertyExtra_taxExempt { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyExtra_taxValue { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyExtra_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyExtra_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyExtra_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyExtra_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblPropertyExtra_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyExtra_deletedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyExtras")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [InverseProperty("tblPropertyExtra")]
    public virtual ICollection<tblPropertySeasonExtra> tblPropertySeasonExtras { get; set; } = new List<tblPropertySeasonExtra>();
}
