using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblTag")]
public partial class tblTag
{
    [Key]
    public int tblTag_id { get; set; }

    public int tblTag_type { get; set; }

    [StringLength(200)]
    public string tblTag_name { get; set; } = null!;

    public string? tblTag_desc { get; set; }

    public int tblTag_icon { get; set; }

    public string? tblTag_propertyCategories { get; set; }

    public int tblTag_order { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblTag_createdUTC { get; set; }

    [StringLength(200)]
    public string? tblTag_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblTag_editedUTC { get; set; }

    [StringLength(200)]
    public string? tblTag_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblTag_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblTag_deletedBy { get; set; }

    [InverseProperty("tblTag")]
    public virtual ICollection<tblContactTag> tblContactTags { get; set; } = new List<tblContactTag>();

    [InverseProperty("tblTag")]
    public virtual ICollection<tblPropertyTag> tblPropertyTags { get; set; } = new List<tblPropertyTag>();
}
