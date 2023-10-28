using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyRelated")]
[Index("tblProperty_id", Name = "IX_tblPropertyRelated_tblProperty_id")]
[Index("tblProperty_relatedId", Name = "IX_tblPropertyRelated_tblProperty_relatedId")]
public partial class tblPropertyRelated
{
    [Key]
    public int tblPropertyRelated_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblPropertyRelated_type { get; set; }

    public int tblProperty_relatedId { get; set; }

    public int tblPropertyRelated_order { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRelated_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRelated_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRelated_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRelated_editedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyRelatedtblProperties")]
    public virtual tblProperty tblProperty { get; set; } = null!;

    [ForeignKey("tblProperty_relatedId")]
    [InverseProperty("tblPropertyRelatedtblProperty_relateds")]
    public virtual tblProperty tblProperty_related { get; set; } = null!;
}
