using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyRoom")]
[Index("tblProperty_id", Name = "IX_tblPropertyRoom_tblProperty_id")]
public partial class tblPropertyRoom
{
    [Key]
    public int tblPropertyRoom_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblPropertyRoom_type { get; set; }

    [StringLength(100)]
    public string? tblPropertyRoom_name { get; set; }

    [StringLength(200)]
    public string? tblPropertyRoom_desc { get; set; }

    public int tblPropertyRoom_bedsDouble { get; set; }

    public int tblPropertyRoom_bedsTwinDouble { get; set; }

    public int tblPropertyRoom_bedsTwin { get; set; }

    public int tblPropertyRoom_bedsSingle { get; set; }

    public int tblPropertyRoom_bedsBunk { get; set; }

    public int tblPropertyRoom_bedsSofa { get; set; }

    public int tblPropertyRoom_bedsChild { get; set; }

    public bool tblPropertyRoom_ensuite { get; set; }

    public bool tblPropertyRoom_accessInside { get; set; }

    public bool tblPropertyRoom_accessOutside { get; set; }

    public string? tblPropertyRoom_note { get; set; }

    public int tblPropertyRoom_order { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRoom_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRoom_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyRoom_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyRoom_editedBy { get; set; }

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyRooms")]
    public virtual tblProperty tblProperty { get; set; } = null!;
}
