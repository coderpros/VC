using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyAvailability")]
[Index("tblProperty_id", Name = "IX_tblPropertyAvailability_tblProperty_id")]
public partial class tblPropertyAvailability
{
    [Key]
    public int tblPropertyAvailability_id { get; set; }

    public int tblProperty_id { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyAvailability_night { get; set; }

    public int tblPropertyAvailability_state { get; set; }

    public string? tblPropertyAvailability_note { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyAvailability_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyAvailability_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyAvailability_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyAvailability_editedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyAvailabilities")]
    public virtual tblProperty tblProperty { get; set; } = null!;
}
