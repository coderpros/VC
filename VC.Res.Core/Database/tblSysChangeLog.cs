using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysChangeLog")]
public partial class tblSysChangeLog
{
    [Key]
    public int tblSysChangeLog_id { get; set; }

    public double tblSysChangeLog_version { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysChangeLog_date { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysChangeLog_applied { get; set; }

    public string? tblSysChangeLog_additions { get; set; }

    public string? tblSysChangeLog_fixes { get; set; }

    public string? tblSysChangeLog_note { get; set; }

    public bool tblSysChangeLog_patched { get; set; }
}
