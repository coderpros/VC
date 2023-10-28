using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblUserSession")]
[Index("tblUser_id", Name = "IX_tblUserSession_tblUser_id")]
public partial class tblUserSession
{
    [Key]
    public int tblUserSession_id { get; set; }

    public int tblUserSession_type { get; set; }

    public int tblUser_id { get; set; }

    [StringLength(50)]
    public string tblUserSession_key1 { get; set; } = null!;

    [StringLength(50)]
    public string tblUserSession_key2 { get; set; } = null!;

    [StringLength(50)]
    public string tblUserSession_key3 { get; set; } = null!;

    [StringLength(50)]
    public string? tblUserSession_key4 { get; set; }

    public bool tblUserSession_authenticated { get; set; }

    public bool tblUserSession_claimed { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserSession_createdUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserSession_createdLocal { get; set; }

    [StringLength(50)]
    public string? tblUserSession_createdIP { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserSession_lastActivityUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserSession_lastActivityLocal { get; set; }

    [StringLength(50)]
    public string? tblUserSession_lastActivityIP { get; set; }

    [ForeignKey("tblUser_id")]
    [InverseProperty("tblUserSessions")]
    public virtual tblUser tblUser { get; set; } = null!;
}
