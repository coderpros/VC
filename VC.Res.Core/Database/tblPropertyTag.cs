using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyTag")]
[Index("tblProperty_id", Name = "IX_tblPropertyTag_tblProperty_id")]
[Index("tblTag_id", Name = "IX_tblPropertyTag_tblTag_id")]
public partial class tblPropertyTag
{
    [Key]
    public int tblPropertyTag_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblTag_id { get; set; }

    public int tblPropertyTag_category { get; set; }

    public string? tblPropertyTag_desc { get; set; }

    public int tblPropertyTag_order { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyTags")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [ForeignKey("tblTag_id")]
    [InverseProperty("tblPropertyTags")]
    public virtual tblTag tblTag { get; set; } = null!;
}
