using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblContact")]
public partial class tblContact
{
    [Key]
    public int tblContact_id { get; set; }

    [StringLength(200)]
    public string? tblContact_zohoId { get; set; }

    [StringLength(100)]
    public string? tblContact_companyName { get; set; }

    [StringLength(20)]
    public string? tblContact_title { get; set; }

    [StringLength(50)]
    public string? tblContact_firstName { get; set; }

    [StringLength(50)]
    public string? tblContact_middleName { get; set; }

    [StringLength(50)]
    public string? tblContact_lastName { get; set; }

    public string? tblContact_websiteURL { get; set; }

    public int tblContact_prefContactMethod { get; set; }

    public string? tblContact_categories { get; set; }

    public string? tblContact_note { get; set; }

    public int tblContact_agentAmountType { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblContact_agentAmount { get; set; }

    public int tblContact_agentPaymentPoint { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblContact_agentPaymentDeposit { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblContact_agentPaymentInterim { get; set; }

    [Column(TypeName = "decimal(18, 6)")]
    public decimal? tblContact_agentPaymentBalance { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContact_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblContact_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblContact_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblContact_editedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? tblContact_deletedUTC { get; set; }

    [StringLength(100)]
    public string? tblContact_deletedBy { get; set; }

    [InverseProperty("tblContact")]
    public virtual ICollection<tblContactAddress> tblContactAddresses { get; set; } = new List<tblContactAddress>();

    [InverseProperty("tblContact")]
    public virtual ICollection<tblContactEmail> tblContactEmails { get; set; } = new List<tblContactEmail>();

    [InverseProperty("tblContact")]
    public virtual ICollection<tblContactTag> tblContactTags { get; set; } = new List<tblContactTag>();

    [InverseProperty("tblContact")]
    public virtual ICollection<tblContactTel> tblContactTels { get; set; } = new List<tblContactTel>();

    [InverseProperty("tblContact")]
    public virtual ICollection<tblPropertyConfig> tblPropertyConfigs { get; set; } = new List<tblPropertyConfig>();

    [InverseProperty("tblContact")]
    public virtual ICollection<tblPropertyContact> tblPropertyContacts { get; set; } = new List<tblPropertyContact>();
}
