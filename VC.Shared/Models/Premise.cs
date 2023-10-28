namespace VC.Shared.Models
{
    public class Premise
    {
        public bool Loaded { get; set; } = false;

        public int Res_Id { get; set; } = 0;

        public int Umb_Id { get; set; } = 0;

        public string Umb_URL { get; set; } = "";

        public string Name { get; set; } = "";

        public string Display_Name { get; set; } = "";

        public string Overview { get; set; } = "";

        public string Address_Line1 { get; set; } = "";
        public string Address_Line2 { get; set; } = "";
        public string Address_Line3 { get; set; } = "";
        public string Address_Town { get; set; } = "";
        public string Address_Region { get; set; } = "";
        public string Address_PostCode { get; set; } = "";
        public Country Res_Country { get; set; } = new();
        public Region Res_Region { get; set; } = new();

        public decimal? Latitude { get; set; } = null;
        public decimal? Longitude { get; set; } = null;

        public int Guests_Max { get; set; } = 0;
        public int Guests_Additional { get; set; } = 0;
        public double? Size { get; set; } = null;
        public int Rooms_NoBathrooms { get; set; } = 0;

        public List<PremiseRoom> Rooms { get; set; } = new();

        public List<PremiseFeature> Features { get; set; } = new();

        public List<PremiseDistance> Distances { get; set; } = new();

        public List<Premise> RentedTogether { get; set; } = new();

        public List<Premise> Alternatives { get; set; } = new();

        public string Website_Pricing_CurrencySymbol { get; set; } = "";
        public Enums.Premises_Premise_WebsiteCurrencyDisplay Website_Pricing_CurrencySymbolDisplay { get; set; } = Enums.Premises_Premise_WebsiteCurrencyDisplay.NotSet;
        public string Website_Pricing_Min { get; set; } = "";
        public string Website_Pricing_Max { get; set; } = "";
        public Enums.Premises_Premise_WebsitePricingType Website_Pricing_Type { get; set; } = Enums.Premises_Premise_WebsitePricingType.NotSet;
    }


    public class PremiseRoom
    {
        public bool Loaded { get; set; } = false;

        public int Res_Id { get; set; } = 0;

        //public int Res_Premise_Id { get; set; } = 0;

        public int Type { get; set; } = (int)Enums.Premises_Room_Type.Unknown;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public int Beds_Double { get; set; } = 0;
        public int Beds_TwinDouble { get; set; } = 0;
        public int Beds_Twin { get; set; } = 0;
        public int Beds_Single { get; set; } = 0;
        public int Beds_Bunk { get; set; } = 0;
        public int Beds_Sofa { get; set; } = 0;
        public int Beds_Child { get; set; } = 0;
        public bool Ensuite { get; set; } = false;

        public bool Access_Inside { get; set; } = false;
        public bool Access_Outside { get; set; } = false;

        public int Order { get; set; } = 0;
    }

    public class PremiseFeature
    {
        public bool Loaded { get; set; } = false;

        public int Res_Id { get; set; } = 0;

        public int Res_Premise_Id { get; set; } = 0;

        public int Res_Tag_Id { get; set; } = 0;

        public int Category { get; set; } = (int)Enums.Premises_Tag_Category.Unknown;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Enums.Common_Tag_Icon Icon { get; set; } = Enums.Common_Tag_Icon.None;

        public int Order { get; set; } = 0;
    }

    public class PremiseDistance
    {
        public bool Loaded { get; set; } = false;

        public int Res_Id { get; set; } = 0;

        //public int Res_Premise_Id { get; set; } = 0;

        public int Type { get; set; } = (int)Enums.Premises_Distance_Type.Unknown;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public double KM { get; set; } = 0;

        public decimal? Latitude { get; set; } = null;

        public decimal? Longitude { get; set; } = null;

        public int? MinBy_Walk { get; set; } = null;

        public int? MinBy_Drive { get; set; } = null;

        public int? MinBy_Boat { get; set; } = null;
    }
}
