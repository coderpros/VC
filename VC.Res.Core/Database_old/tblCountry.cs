using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblCountry")]
public partial class tblCountry
{
    [Key]
    public int tblCountry_id { get; set; }

    public int? tblCountry_websiteId { get; set; }

    [StringLength(200)]
    public string tblCountry_name { get; set; } = null!;

    [StringLength(2)]
    public string? tblCountry_A2 { get; set; }

    [StringLength(3)]
    public string? tblCountry_A3 { get; set; }

    public int? tblCountry_number { get; set; }

    public int tblCountry_order { get; set; }

    public bool tblCountry_enabled { get; set; }

    [Column(TypeName = "decimal(8, 4)")]
    public decimal? tblCountry_taxRate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblCountry_createdUtc { get; set; }

    [StringLength(100)]
    public string? tblCountry_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblCountry_editedUtc { get; set; }

    [StringLength(100)]
    public string? tblCountry_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblCountry_deletedUtc { get; set; }

    [StringLength(100)]
    public string? tblCountry_deletedBy { get; set; }

    [InverseProperty("tblCountry")]
    public virtual ICollection<tblContactAddress> tblContactAddresses { get; set; } = new List<tblContactAddress>();

    [InverseProperty("tblCountry")]
    public virtual ICollection<tblProperty> tblProperties { get; set; } = new List<tblProperty>();

    [InverseProperty("tblCountry")]
    public virtual ICollection<tblRegion> tblRegions { get; set; } = new List<tblRegion>();
}
