using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblUser")]
[Index("tblUser_email", Name = "IX_tblUser_tblUser_email")]
public partial class tblUser
{
    [Key]
    public int tblUser_id { get; set; }

    [StringLength(100)]
    public string tblUser_email { get; set; } = null!;

    [StringLength(100)]
    public string? tblUser_firstName { get; set; }

    [StringLength(100)]
    public string? tblUser_lastName { get; set; }

    [StringLength(100)]
    public string? tblUser_pwdSalt { get; set; }

    [StringLength(100)]
    public string? tblUser_pwd { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_pwdLastChangedUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_pwdLastChangedLocal { get; set; }

    public bool tblUser_twoFAEnabled { get; set; }

    public int tblUser_twoFAMethod { get; set; }

    [StringLength(100)]
    public string? tblUser_telMobile { get; set; }

    public bool tblUser_telMobileVerified { get; set; }

    public bool tblUser_accessSysAdmin { get; set; }

    public bool tblUser_enabled { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_lastLoginUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_lastLoginLocal { get; set; }

    [StringLength(100)]
    public string? tblUser_lastLoginIP { get; set; }

    public int tblUser_failedLoginTotal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_failedLoginLastUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_failedLoginLastLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_failedLoginLockUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_failedLoginLockLocal { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUser_createdUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUser_createdLocal { get; set; }

    [StringLength(200)]
    public string? tblUser_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUser_editedUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblUser_editedLocal { get; set; }

    [StringLength(200)]
    public string? tblUser_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_deletedUTC { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblUser_deletedLocal { get; set; }

    [StringLength(200)]
    public string? tblUser_deletedBy { get; set; }

    [InverseProperty("tblUser")]
    public virtual ICollection<tblSysEmail> tblSysEmails { get; set; } = new List<tblSysEmail>();

    [InverseProperty("tblUser")]
    public virtual ICollection<tblUserActivity> tblUserActivities { get; set; } = new List<tblUserActivity>();

    [InverseProperty("tblUser")]
    public virtual ICollection<tblUserAuthCode> tblUserAuthCodes { get; set; } = new List<tblUserAuthCode>();

    [InverseProperty("tblUser")]
    public virtual ICollection<tblUserIP> tblUserIPs { get; set; } = new List<tblUserIP>();

    [InverseProperty("tblUser")]
    public virtual ICollection<tblUserSession> tblUserSessions { get; set; } = new List<tblUserSession>();
}
