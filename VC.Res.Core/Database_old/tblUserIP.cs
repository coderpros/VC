using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblUserIP")]
[Index("tblUser_id", Name = "IX_tblUserIP_tblUser_id")]
public partial class tblUserIP
{
    [Key]
    public int tblUserIP_id { get; set; }

    public int tblUser_id { get; set; }

    [StringLength(100)]
    public string? tblUserIP_ipAddress { get; set; }

    public bool tblUserIP_authorised { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserIP_lastLoginUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserIP_lastLoginLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserIP_createdUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserIP_createdLocal { get; set; }

    [ForeignKey("tblUser_id")]
    [InverseProperty("tblUserIPs")]
    public virtual tblUser tblUser { get; set; } = null!;
}
