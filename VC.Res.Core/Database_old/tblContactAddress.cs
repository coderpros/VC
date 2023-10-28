using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblContactAddress")]
[Index("tblContact_id", Name = "IX_tblContactAddress_tblContact_id")]
[Index("tblCountry_id", Name = "IX_tblContactAddress_tblCountry_id")]
public partial class tblContactAddress
{
    [Key]
    public int tblContactAddress_id { get; set; }

    public int tblContact_id { get; set; }

    [StringLength(100)]
    public string tblContactAddress_line1 { get; set; } = null!;

    [StringLength(100)]
    public string? tblContactAddress_line2 { get; set; }

    [StringLength(100)]
    public string? tblContactAddress_line3 { get; set; }

    [StringLength(100)]
    public string tblContactAddress_town { get; set; } = null!;

    [StringLength(100)]
    public string? tblContactAddress_region { get; set; }

    [StringLength(30)]
    public string? tblContactAddress_postCode { get; set; }

    public int tblCountry_id { get; set; }

    public bool tblContactAddress_primary { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactAddress_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblContactAddress_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactAddress_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblContactAddress_editedBy { get; set; }

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblContactAddresses")]
    public virtual tblContact tblContact { get; set; } = null!;

    [ForeignKey("tblCountry_id")]
    [InverseProperty("tblContactAddresses")]
    public virtual tblCountry tblCountry { get; set; } = null!;
}
