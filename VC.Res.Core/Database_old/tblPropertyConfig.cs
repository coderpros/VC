using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyConfig")]
[Index("tblContact_id", Name = "IX_tblPropertyConfig_tblContact_id")]
[Index("tblCurrency_id", Name = "IX_tblPropertyConfig_tblCurrency_id")]
[Index("tblPropertyConfig_parentId", Name = "IX_tblPropertyConfig_tblPropertyConfig_parentId")]
[Index("tblPropertySeason_id", Name = "IX_tblPropertyConfig_tblPropertySeason_id")]
[Index("tblProperty_id", Name = "IX_tblPropertyConfig_tblProperty_id")]
public partial class tblPropertyConfig
{
    [Key]
    public int tblPropertyConfig_id { get; set; }

    public int? tblPropertyConfig_parentId { get; set; }

    public int? tblContact_id { get; set; }

    public int? tblProperty_id { get; set; }

    public int? tblPropertySeason_id { get; set; }

    public bool tblPropertyConfig_descForQuoteInh { get; set; }

    public string? tblPropertyConfig_descForQuote { get; set; }

    public bool tblPropertyConfig_houseRulesInh { get; set; }

    public string? tblPropertyConfig_houseRules { get; set; }

    public bool tblPropertyConfig_inclusionsInh { get; set; }

    public string? tblPropertyConfig_inclusions { get; set; }

    public bool tblPropertyConfig_defAvailStateInh { get; set; }

    public int tblPropertyConfig_defAvailState { get; set; }

    public bool tblPropertyConfig_reqBookingApvlInh { get; set; }

    public bool tblPropertyConfig_reqBookingApvl { get; set; }

    public bool tblPropertyConfig_priceEntryModeInh { get; set; }

    public int tblPropertyConfig_priceEntryMode { get; set; }

    public bool tblPropertyConfig_currencyInh { get; set; }

    public int? tblCurrency_id { get; set; }

    public bool tblPropertyConfig_taxInh { get; set; }

    public bool tblPropertyConfig_taxExempt { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_taxValue { get; set; }

    [StringLength(200)]
    public string? tblPropertyConfig_taxNo { get; set; }

    public bool tblPropertyConfig_bankInh { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAccName { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAccNo { get; set; }

    [StringLength(50)]
    public string? tblPropertyConfig_bankAccSort { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAccIBAN { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAccBIC { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddress1 { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddress2 { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddress3 { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddressTown { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddressCounty { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddressPost { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_bankAddressCountry { get; set; }

    public bool tblPropertyConfig_commissionInh { get; set; }

    public int tblPropertyConfig_commissionAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_commissionAmount { get; set; }

    public string? tblPropertyConfig_commissionNote { get; set; }

    public bool tblPropertyConfig_checkinInh { get; set; }

    public TimeSpan? tblPropertyConfig_checkin { get; set; }

    public bool tblPropertyConfig_checkoutInh { get; set; }

    public TimeSpan? tblPropertyConfig_checkout { get; set; }

    public bool tblPropertyConfig_changeOverDayInh { get; set; }

    public int? tblPropertyConfig_changeOverDay { get; set; }

    public bool tblPropertyConfig_minRentalInh { get; set; }

    public int? tblPropertyConfig_minRentalDays { get; set; }

    public string? tblPropertyConfig_minRentalNote { get; set; }

    public bool tblPropertyConfig_nightlyPriceInh { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_nightlyPrice { get; set; }

    public bool tblPropertyConfig_paySchInh { get; set; }

    public bool tblPropertyConfig_paySchDeposReq { get; set; }

    public int tblPropertyConfig_paySchDeposAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_paySchDeposAmount { get; set; }

    public bool tblPropertyConfig_paySchInterReq { get; set; }

    public int tblPropertyConfig_paySchInterAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_paySchInterAmount { get; set; }

    public int? tblPropertyConfig_paySchInterDays { get; set; }

    public int? tblPropertyConfig_paySchBalDays { get; set; }

    public bool tblPropertyConfig_paySecDeposInh { get; set; }

    public bool tblPropertyConfig_paySecDeposReq { get; set; }

    public int tblPropertyConfig_paySecDeposAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblPropertyConfig_paySecDeposAmount { get; set; }

    public int tblPropertyConfig_paySecDeposCalcFrom { get; set; }

    public int? tblPropertyConfig_paySecDeposDaysBefore { get; set; }

    public int? tblPropertyConfig_paySecDeposDaysAfter { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyConfig_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyConfig_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyConfig_editedBy { get; set; }

    [InverseProperty("tblPropertyConfig_parent")]
    public virtual ICollection<tblPropertyConfig> InversetblPropertyConfig_parent { get; set; } = new List<tblPropertyConfig>();

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblPropertyConfigs")]
    public virtual tblContact? tblContact { get; set; }

    [ForeignKey("tblCurrency_id")]
    [InverseProperty("tblPropertyConfigs")]
    public virtual tblCurrency? tblCurrency { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyConfigs")]
    public virtual tblProperty? tblProperty { get; set; }

    [ForeignKey("tblPropertyConfig_parentId")]
    [InverseProperty("InversetblPropertyConfig_parent")]
    public virtual tblPropertyConfig? tblPropertyConfig_parent { get; set; }

    [ForeignKey("tblPropertySeason_id")]
    [InverseProperty("tblPropertyConfigs")]
    public virtual tblPropertySeason? tblPropertySeason { get; set; }
}
