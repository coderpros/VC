using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblUserAuthCode")]
[Index("tblUser_id", Name = "IX_tblUserAuthCode_tblUser_id")]
public partial class tblUserAuthCode
{
    [Key]
    public int tblUserAuthCode_id { get; set; }

    public int tblUser_id { get; set; }

    [StringLength(6)]
    public string tblUserAuthCode_code { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime tblUserAuthCode_expiresUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserAuthCode_expiresLocal { get; set; }

    [ForeignKey("tblUser_id")]
    [InverseProperty("tblUserAuthCodes")]
    public virtual tblUser tblUser { get; set; } = null!;
}
