using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblContactTag")]
[Index("tblContact_id", Name = "IX_tblContactTag_tblContact_id")]
[Index("tblTag_id", Name = "IX_tblContactTag_tblTag_id")]
public partial class tblContactTag
{
    [Key]
    public int tblContactTag_id { get; set; }

    public int tblContact_id { get; set; }

    public int tblTag_id { get; set; }

    public int tblContactTag_order { get; set; }

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblContactTags")]
    public virtual tblContact tblContact { get; set; } = null!;

    [ForeignKey("tblTag_id")]
    [InverseProperty("tblContactTags")]
    public virtual tblTag tblTag { get; set; } = null!;
}
