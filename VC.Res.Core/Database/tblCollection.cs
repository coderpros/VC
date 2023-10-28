namespace VC.Res.Core.Database;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("tblCollection")]
public partial class tblCollection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int tblCollection_id { get; set; }

    [Required]
    [StringLength(200)]
    public string tblCollection_name { get; set; } = null;
    public string? tblCollection_desc { get; set; }

    [DefaultValue(false)]
    public bool tblCollection_enabled { get; set; } = false;

    [DefaultValue(false)]
    public bool tblCollection_saveToUmbraco { get; set; } = false;

    [Column(TypeName = "datetime")]
    public DateTime tblCollection_createdUTC { get; set; }

    [StringLength(200)]
    public string? tblCollection_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblCollection_editedUTC { get; set; }

    [StringLength(200)]
    public string? tblCollection_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblCollection_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblCollection_deletedBy { get; set; }

    [InverseProperty("tblCollection")]
    public virtual ICollection<tblPropertyCollection> tblPropertyCollections { get; set; } = new List<tblPropertyCollection>();
}
