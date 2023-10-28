using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblUserActivity")]
[Index("tblSysAudit_id", Name = "IX_tblUserActivity_tblSysAudit_id")]
[Index("tblUser_id", Name = "IX_tblUserActivity_tblUser_id")]
public partial class tblUserActivity
{
    [Key]
    public int tblUserActivity_id { get; set; }

    public int? tblUser_id { get; set; }

    [StringLength(200)]
    public string? tblUserActivity_email { get; set; }

    public int tblUserActivity_actionGroup { get; set; }

    public int tblUserActivity_actionType { get; set; }

    [StringLength(200)]
    public string? tblUserActivity_actionText { get; set; }

    public int? tblUserActivity_refItemId1 { get; set; }

    public int? tblUserActivity_refItemId2 { get; set; }

    public int? tblUserActivity_refItemId3 { get; set; }

    public int? tblUserActivity_refItemId4 { get; set; }

    public int? tblUserActivity_refItemId5 { get; set; }

    public int? tblSysAudit_id { get; set; }

    public bool tblUserActivity_success { get; set; }

    public string? tblUserActivity_note { get; set; }

    [StringLength(50)]
    public string? tblUserActivity_ipAddress { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserActivity_createdUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUserActivity_createdLocal { get; set; }

    [ForeignKey("tblSysAudit_id")]
    [InverseProperty("tblUserActivities")]
    public virtual tblSysAudit? tblSysAudit { get; set; }

    [ForeignKey("tblUser_id")]
    [InverseProperty("tblUserActivities")]
    public virtual tblUser? tblUser { get; set; }
}
