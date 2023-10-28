using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Keyless]
public partial class vwPropertyCollection
{
    public int tblPropertyCollection_id { get; set; }

    public int tblProperty_id { get; set; }

    public int tblCollection_id { get; set; }

    [StringLength(200)]
    public string tblCollection_name { get; set; } = null!;

    public string? tblCollection_desc { get; set; }
}