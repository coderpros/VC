using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VC.Res.Core.Database;

[Table("tblPropertyContact")]
[Index("tblContact_id", Name = "IX_tblPropertyContact_tblContact_id")]
[Index("tblPropertyGroup_id", Name = "IX_tblPropertyContact_tblPropertyGroup_id")]
[Index("tblProperty_id", Name = "IX_tblPropertyContact_tblProperty_id")]
public partial class tblPropertyContact
{
    [Key]
    public int tblPropertyContact_id { get; set; }

    public int? tblProperty_id { get; set; }

    public int? tblPropertyGroup_id { get; set; }

    public int tblContact_id { get; set; }

    public bool tblPropertyContact_configPrimary { get; set; }

    public string? tblPropertyContact_categories { get; set; }

    public bool tblPropertyContact_papmInfo { get; set; }

    public bool tblPropertyContact_papmRates { get; set; }

    public bool tblPropertyContact_papmAvailability { get; set; }

    public bool tblPropertyContact_papmBookings { get; set; }

    public bool tblPropertyContact_papmBookingConfirmation { get; set; }

    public bool tblPropertyContact_papmRemitSlip { get; set; }

    public bool tblPropertyContact_notifiInfo { get; set; }

    public bool tblPropertyContact_notifiRates { get; set; }

    public bool tblPropertyContact_notifiAvailability { get; set; }

    public bool tblPropertyContact_notifiBookings { get; set; }

    public bool tblPropertyContact_notifiBookingConfirmation { get; set; }

    public bool tblPropertyContact_notifiRemitSlip { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyContact_createdUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyContact_createdBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime tblPropertyContact_editedUTC { get; set; }

    [StringLength(100)]
    public string? tblPropertyContact_editedBy { get; set; }

    [ForeignKey("tblContact_id")]
    [InverseProperty("tblPropertyContacts")]
    public virtual tblContact tblContact { get; set; } = null!;

    [ForeignKey("tblProperty_id")]
    [InverseProperty("tblPropertyContacts")]
    public virtual tblProperty? tblProperty { get; set; }

    [ForeignKey("tblPropertyGroup_id")]
    [InverseProperty("tblPropertyContacts")]
    public virtual tblPropertyGroup? tblPropertyGroup { get; set; }
}
