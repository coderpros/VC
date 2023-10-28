using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblCurrency")]
public partial class tblCurrency
{
    [Key]
    public int tblCurrency_id { get; set; }

    [StringLength(100)]
    public string tblCurrency_name { get; set; } = null!;

    [StringLength(5)]
    public string tblCurrency_code { get; set; } = null!;

    [StringLength(10)]
    public string tblCurrency_symbol { get; set; } = null!;

    public bool tblCurrency_symbolAfter { get; set; }

    public bool tblCurrency_default { get; set; }

    public int tblCurrency_order { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblCurrency_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblCurrency_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblCurrency_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblCurrency_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblCurrency_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblCurrency_deletedBy { get; set; }

    [InverseProperty("tblCurrency")]
    public virtual ICollection<tblPropertyConfig> tblPropertyConfigs { get; set; } = new List<tblPropertyConfig>();
}
