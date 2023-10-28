using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysSetting")]
public partial class tblSysSetting
{
    [Key]
    public int tblSysSetting_id { get; set; }

    public int tblSysSetting_type { get; set; }

    [StringLength(200)]
    public string tblSysSetting_key { get; set; } = null!;

    public string? tblSysSetting_value { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysSetting_createdUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysSetting_editedUtc { get; set; }
}
