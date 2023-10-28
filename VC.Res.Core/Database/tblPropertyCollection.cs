namespace VC.Res.Core.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("tblPropertyCollection")]
[Index("tblProperty_id", Name = "IX_tblPropertyCollection_tblProperty_id")]
[Index("tblCollection_id", Name = "IX_tblPropertyCollection_tblCollection_id")]
public partial class tblPropertyCollection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int tblPropertyCollection_id { get; set; }

    public int tblCollection_id { get; set; }

    public int tblProperty_id { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyCollections")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [ForeignKey("tblCollection_id")]
    [InverseProperty("tblPropertyCollections")]
    public virtual tblCollection tblCollection { get; set; } = null!;
}