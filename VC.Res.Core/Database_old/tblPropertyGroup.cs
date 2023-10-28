using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyGroup")]
public partial class tblPropertyGroup
{
    [Key]
    public int tblPropertyGroup_id { get; set; }

    [StringLength(100)]
    public string tblPropertyGroup_name { get; set; } = null!;

    public string? tblPropertyGroup_desc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyGroup_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyGroup_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyGroup_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyGroup_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblPropertyGroup_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyGroup_deletedBy { get; set; }

    [InverseProperty("tblPropertyGroup")]
    public virtual ICollection<tblProperty> tblProperties { get; set; } = new List<tblProperty>();

    [InverseProperty("tblPropertyGroup")]
    public virtual ICollection<tblPropertyContact> tblPropertyContacts { get; set; } = new List<tblPropertyContact>();
}
