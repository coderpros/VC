using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysCacheMonitor")]
public partial class tblSysCacheMonitor
{
    [Key]
    [StringLength(300)]
    public string tblSysCacheMonitor_table { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime tblSysCacheMonitor_created { get; set; }

    public int tblSysCacheMonitor_changeId { get; set; }
}
