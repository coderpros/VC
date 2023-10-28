using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblContactTel")]
[Index("tblContact_id", Name = "IX_tblContactTel_tblContact_id")]
public partial class tblContactTel
{
    [Key]
    public int tblContactTel_id { get; set; }

    public int tblContact_id { get; set; }

    [StringLength(10)]
    public string tblContactTel_countryCode { get; set; } = null!;

    [StringLength(50)]
    public string tblContactTel_no { get; set; } = null!;

    public bool tblContactTel_primary { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactTel_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblContactTel_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactTel_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblContactTel_editedBy { get; set; }

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblContactTels")]
    public virtual tblContact tblContact { get; set; } = null!;
}
