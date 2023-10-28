using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysAudit")]
public partial class tblSysAudit
{
    [Key]
    public int tblSysAudit_id { get; set; }

    [StringLength(200)]
    public string tblSysAudit_type { get; set; } = null!;

    public int tblSysAudit_foreignId { get; set; }

    public int tblSysAudit_action { get; set; }

    public string? tblSysAudit_newData { get; set; }

    public string? tblSysAudit_oldData { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysAudit_createdUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysAudit_createdLocal { get; set; }

    public string? tblSysAudit_createdBy { get; set; }

    [InverseProperty("tblSysAudit")]
    public virtual ICollection<tblUserActivity> tblUserActivities { get; set; } = new List<tblUserActivity>();
}
