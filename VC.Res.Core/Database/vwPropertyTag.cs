using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Keyless]
public partial class vwPropertyTag
{
    public int tblPropertyTag_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblTag_id { get; set; }

    public int tblTag_type { get; set; }

    [StringLength(200)]
    public string tblTag_name { get; set; } = null!;

    public string? tblTag_desc { get; set; }

    public int tblTag_icon { get; set; }

    public int tblPropertyTag_category { get; set; }

    public string? tblPropertyTag_desc { get; set; }

    public int tblPropertyTag_order { get; set; }
}
