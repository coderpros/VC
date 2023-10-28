using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblProperty")]
[Index("tblCountry_id", Name = "IX_tblProperty_tblCountry_id")]
[Index("tblPropertyGroup_id", Name = "IX_tblProperty_tblPropertyGroup_id")]
[Index("tblRegion_id", Name = "IX_tblProperty_tblRegion_id")]
public partial class tblProperty
{
    [Key]
    public int tblProperty_id { get; set; }

    [StringLength(200)]
    public string tblProperty_name { get; set; } = null!;

    [StringLength(200)]
    public string? tblProperty_displayName { get; set; }

    public int? tblProperty_websiteId { get; set; }

    [StringLength(200)]
    public string? tblProperty_websiteURL { get; set; }

    public string? tblProperty_overview { get; set; }

    public string? tblProperty_otherWebsiteURLs { get; set; }

    public int tblProperty_channel { get; set; }

    [StringLength(100)]
    public string tblProperty_addressLine1 { get; set; } = null!;

    [StringLength(100)]
    public string? tblProperty_addressLine2 { get; set; }

    [StringLength(100)]
    public string? tblProperty_addressLine3 { get; set; }

    [StringLength(100)]
    public string tblProperty_addressTown { get; set; } = null!;

    [StringLength(100)]
    public string? tblProperty_addressRegion { get; set; }

    [StringLength(30)]
    public string? tblProperty_addressPostCode { get; set; }

    public int? tblCountry_id { get; set; }

    public int? tblRegion_id { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? tblProperty_lat { get; set; }

    [Column(TypeName = "decimal(9, 6)")]
    public decimal? tblProperty_long { get; set; }

    public int tblProperty_maxGuests { get; set; }

    public int tblProperty_maxGuestsAdditional { get; set; }

    public double? tblProperty_size { get; set; }

    public int tblProperty_noBathrooms { get; set; }

    public int? tblPropertyGroup_id { get; set; }

    public bool tblProperty_groupUseContacts { get; set; }

    [StringLength(100)]
    public string? tblProperty_licenceNo { get; set; }

    [StringLength(10)]
    public string? tblProperty_webPriceCurrencySymb { get; set; }

    public int tblProperty_webPriceCurrencyDisplay { get; set; }

    [StringLength(100)]
    public string? tblProperty_webPriceMin { get; set; }

    [StringLength(100)]
    public string? tblProperty_webPriceMax { get; set; }

    public int tblProperty_webPriceType { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblProperty_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblProperty_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblProperty_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblProperty_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblProperty_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblProperty_deletedBy { get; set; }

    [ForeignKey("tblCountry_id")]
    [InverseProperty("tblProperties")]
    public virtual tblCountry? tblCountry { get; set; }

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyAvailability> tblPropertyAvailabilities { get; set; } = new List<tblPropertyAvailability>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyConfig> tblPropertyConfigs { get; set; } = new List<tblPropertyConfig>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyContact> tblPropertyContacts { get; set; } = new List<tblPropertyContact>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyDistance> tblPropertyDistances { get; set; } = new List<tblPropertyDistance>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyExtra> tblPropertyExtras { get; set; } = new List<tblPropertyExtra>();

    [ForeignKey("tblPropertyGroup_id")]
    [InverseProperty("tblProperties")]
    public virtual tblPropertyGroup? tblPropertyGroup { get; set; }

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyRate> tblPropertyRates { get; set; } = new List<tblPropertyRate>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyRelated> tblPropertyRelatedtblProperties { get; set; } = new List<tblPropertyRelated>();

    [InverseProperty("tblProperty_related")]
    public virtual ICollection<tblPropertyRelated> tblPropertyRelatedtblProperty_relateds { get; set; } = new List<tblPropertyRelated>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyRoom> tblPropertyRooms { get; set; } = new List<tblPropertyRoom>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertySeason> tblPropertySeasons { get; set; } = new List<tblPropertySeason>();

    [InverseProperty("tblProperty")]
    public virtual ICollection<tblPropertyTag> tblPropertyTags { get; set; } = new List<tblPropertyTag>();

    [ForeignKey("tblRegion_id")]
    [InverseProperty("tblProperties")]
    public virtual tblRegion? tblRegion { get; set; }
}
