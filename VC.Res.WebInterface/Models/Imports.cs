using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VC.Res.WebInterface.Models.Imports
{
    public class Row
    {
        public int RowIndex { get; set; } = 0;

        public List<string> Warnings { get; set; } = new List<string>();

        public bool Imported { get; set; } = false;
    }

    public class Room : Row
    {
        public int Premise_Id { get; set; } = 0;
        public string Premise_Name { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Note { get; set; } = "";
        public int Beds_Double { get; set; } = 0;
        public int Beds_TwinDouble { get; set; } = 0;
        public int Beds_Twin { get; set; } = 0;
        public int Beds_Single { get; set; } = 0;
        public int Beds_Bunk { get; set; } = 0;
        public int Beds_Sofa { get; set; } = 0;
        public int Beds_Child { get; set; } = 0;
        public bool Ensuite { get; set; } = false;
    }

    public class Nearby : Row
    {
        public int Premise_Id { get; set; } = 0;
        public string Premise_Name { get; set; } = "";
        public VC.Shared.Enums.Premises_Distance_Type Type { get; set; } = VC.Shared.Enums.Premises_Distance_Type.Unknown;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double KM { get; set; } = 0;
        public decimal? Latitude { get; set; } = null;
        public decimal? Longitude { get; set; } = null;
        public int? MinBy_Walk { get; set; } = null;
        public int? MinBy_Drive { get; set; } = null;
        public int? MinBy_Boat { get; set; } = null;
    }

    public class Property : Row
    {
        
        public int Premise_Id { get; set; }
        
        public string tblProperty_name { get; set; } = null!;

        public string? tblProperty_displayName { get; set; }

        public int? tblProperty_websiteId { get; set; }

        public string? tblProperty_websiteURL { get; set; }

        public string? tblProperty_overview { get; set; }

        public string? tblProperty_otherWebsiteURLs { get; set; }

        public int tblProperty_channel { get; set; }

        public string tblProperty_addressLine1 { get; set; } = null!;

        public string? tblProperty_addressLine2 { get; set; }

        public string? tblProperty_addressLine3 { get; set; }

        public string tblProperty_addressTown { get; set; } = null!;

        public string? tblProperty_addressRegion { get; set; }

        public string? tblProperty_addressPostCode { get; set; }

        public int? tblCountry_id { get; set; }

        public int? tblRegion_id { get; set; }

        public decimal? tblProperty_lat { get; set; }

        public decimal? tblProperty_long { get; set; }

        public int tblProperty_maxGuests { get; set; }

        public int tblProperty_maxGuestsAdditional { get; set; }

        public double? tblProperty_size { get; set; }

        public int tblProperty_noBathrooms { get; set; }

        public int? tblPropertyGroup_id { get; set; }

        public bool tblProperty_groupUseContacts { get; set; }

        public string? tblProperty_licenceNo { get; set; }

        public string? tblProperty_webPriceCurrencySymb { get; set; }

        public int tblProperty_webPriceCurrencyDisplay { get; set; }

        public string? tblProperty_webPriceMin { get; set; }

        public string? tblProperty_webPriceMax { get; set; }

        public int tblProperty_webPriceType { get; set; }

        public DateTime tblProperty_createdUTC { get; set; }

        public string? tblProperty_createdBy { get; set; }

        public DateTime tblProperty_editedUTC { get; set; }

        public string? tblProperty_editedBy { get; set; }

        public DateTime? tblProperty_deletedUTC { get; set; }

        public string? tblProperty_deletedBy { get; set; }
    }
    
}
