using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblSysError")]
public partial class tblSysError
{
    [Key]
    public int tblSysError_id { get; set; }

    [StringLength(100)]
    public string? tblSysError_logger { get; set; }

    [StringLength(100)]
    public string? tblSysError_level { get; set; }

    public int tblSysErorr_priority { get; set; }

    public string? tblSysError_class { get; set; }

    public string? tblSysError_method { get; set; }

    public string? tblSysError_parameters { get; set; }

    public string? tblSysError_message { get; set; }

    public string? tblSysError_stackTrace { get; set; }

    public string? tblSysError_innerEx { get; set; }

    public string? tblSysError_other { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblSysError_occurredUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblSysError_dismissedUTC { get; set; }

    [StringLength(200)]
    public string? tblSysError_dismissedBy { get; set; }
}
