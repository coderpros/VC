using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblContactEmail")]
[Index("tblContact_id", Name = "IX_tblContactEmail_tblContact_id")]
public partial class tblContactEmail
{
    [Key]
    public int tblContactEmail_id { get; set; }

    public int tblContact_id { get; set; }

    [StringLength(200)]
    public string tblContactEmail_address { get; set; } = null!;

    public bool tblContactEmail_primary { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactEmail_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblContactEmail_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContactEmail_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblContactEmail_editedBy { get; set; }

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblContactEmails")]
    public virtual tblContact tblContact { get; set; } = null!;
}
