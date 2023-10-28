using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyRate")]
[Index("tblPropertyRate_parentId", Name = "IX_tblPropertyRate_tblPropertyRate_parentId")]
[Index("tblPropertySeason_id", Name = "IX_tblPropertyRate_tblPropertySeason_id")]
[Index("tblProperty_id", Name = "IX_tblPropertyRate_tblProperty_id")]
public partial class tblPropertyRate
{
    [Key]
    public int tblPropertyRate_id { get; set; }

    public int tblProperty_id { get; set; }

    public int? tblPropertySeason_id { get; set; }

    public int? tblPropertyRate_parentId { get; set; }

    public bool tblPropertyRate_provisional { get; set; }

    public bool tblPropertyRate_reqReview { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRate_dateArrive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRate_dateDepart { get; set; }

    public int tblPropertyRate_noNights { get; set; }

    public int tblPropertyRate_minPartySize { get; set; }

    public bool tblPropertyRate_available { get; set; }

    public bool tblPropertyRate_pricePOA { get; set; }

    public int tblPropertyRate_priceEntryMode { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertyRate_price { get; set; }

    public int tblPropertyRate_commissionAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertyRate_commissionAmount { get; set; }

    public string? tblPropertyRate_commissionNote { get; set; }

    public bool tblPropertyRate_taxExempt { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertyRate_taxValue { get; set; }

    public bool tblPropertyRate_discount { get; set; }

    public int tblPropertyRate_discountNights { get; set; }

    public int tblPropertyRate_discountEntryMode { get; set; }

    public int tblPropertyRate_discountAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal tblPropertyRate_discountAmount { get; set; }

    public string? tblPropertyRate_discountNote { get; set; }

    public string? tblPropertyRate_extraExcludes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRate_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRate_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRate_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRate_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblPropertyRate_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRate_deletedBy { get; set; }

    [InverseProperty("tblPropertyRate_parent")]
    public virtual ICollection<tblPropertyRate> InversetblPropertyRate_parent { get; set; } = new List<tblPropertyRate>();

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyRates")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [ForeignKey("tblPropertyRate_parentId")]
    [InverseProperty("InversetblPropertyRate_parent")]
    public virtual tblPropertyRate? tblPropertyRate_parent { get; set; }

    [ForeignKey("tblPropertySeason_id")]
    [InverseProperty("tblPropertyRates")]
    public virtual tblPropertySeason? tblPropertySeason { get; set; }
}
