using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysEmail")]
[Index("tblUser_id", Name = "IX_tblSysEmail_tblUser_id")]
public partial class tblSysEmail
{
    [Key]
    public int tblSysEmail_id { get; set; }

    public int tblSysEmail_type { get; set; }

    [StringLength(50)]
    public string tblSysEmail_key { get; set; } = null!;

    public int? tblUser_id { get; set; }

    public int? tblSysEmail_foreignRef { get; set; }

    public string? tblSysEmail_to { get; set; }

    public string? tblSysEmail_cc { get; set; }

    public string? tblSysEmail_bcc { get; set; }

    [StringLength(200)]
    public string? tblSysEmail_subject { get; set; }

    [StringLength(200)]
    public string? tblSysEmail_fromName { get; set; }

    [StringLength(200)]
    public string? tblSysEmail_fromAddress { get; set; }

    public string? tblSysEmail_template { get; set; }

    public string? tblSysEmail_variables { get; set; }

    public string? tblSysEmail_attachments { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysEmail_createdUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysEmail_createdLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_sentUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_sentLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_usedUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_usedLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_expiresUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysEmail_expiresLocal { get; set; }

    [ForeignKey("tblUser_id")]
    [InverseProperty("tblSysEmails")]
    public virtual tblUser? tblUser { get; set; }
}
