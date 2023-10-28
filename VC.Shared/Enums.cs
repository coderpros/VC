namespace VC.Shared
{
    public class Enums
    {
        public enum Common_Tag_Icon
        {
            None = 0,
            DoubleBed = 10,
            SingleBed = 20,
            Kettle = 30,
            Sofa = 40,
            Briefcase = 50,
            Rocket = 60,
            WashingMachine = 70,
            CookingStation = 80,
            TennisRacket = 90,
            Garden = 100,
            Car = 110,
            Grass = 120,
            KnifeFolk = 130,
            ChefHat = 140,
            Hover = 150,
            Bell = 160,
            Flower = 170,
            Pool = 180,
            SoapDispenser = 190,
            FoldedTowels = 200,
            HangingTowels = 210
        };

        public enum Premises_Distance_Type
        {
            Unknown = 0,
            Beach = 10,
            Airport = 20,
            Town = 30,
            Restaurant = 40,
            Supermarket = 50,
            GolfCourse = 60,
            Other = 70
        };

        public enum Premises_Premise_WebsiteCurrencyDisplay
        {
            NotSet = 0,
            Before = 10,
            After = 20
        };

        public enum Premises_Premise_WebsitePricingType
        {
            NotSet = 0,
            Day = 10,
            Week = 20
        };

        public enum Premises_Room_Type
        {
            Unknown = 0,
            Bedroom = 10
        };

        public enum Premises_Tag_Category
        {
            Unknown = 0,
            LivingSpace = 20,
            IndoorFeatures = 30,
            Outdoors = 10,
            OutdoorFeatures = 40,
            IncludedFeatures = 50,
            ServicesOnRequest = 70,
            OtherInformation = 60
        };

        public static string Label(Enum enumValue)
        {
            // We only need to add elements where the enum text isn't right, for example when a space is required or it needs to be named something else
            return enumValue switch
            {
                Common_Tag_Icon.DoubleBed => "Double bed",
                Common_Tag_Icon.SingleBed => "Single bed",
                Common_Tag_Icon.WashingMachine => "Washing machine",
                Common_Tag_Icon.CookingStation => "BBQ",
                Common_Tag_Icon.TennisRacket => "Tennis racket",
                Common_Tag_Icon.KnifeFolk => "Knife and folk",
                Common_Tag_Icon.ChefHat => "Chef hat",
                Common_Tag_Icon.SoapDispenser => "Soap dispenser",
                Common_Tag_Icon.FoldedTowels => "Towels folded",
                Common_Tag_Icon.HangingTowels => "Towels hanging",

                Premises_Premise_WebsiteCurrencyDisplay.NotSet => "Not set",

                Premises_Distance_Type.GolfCourse => "Golf course",

                Premises_Premise_WebsitePricingType.NotSet => "Not set",

                Premises_Tag_Category.LivingSpace => "Living space",
                Premises_Tag_Category.IndoorFeatures => "Indoor feature",
                Premises_Tag_Category.OutdoorFeatures => "Outdoor feature",
                Premises_Tag_Category.IncludedFeatures => "Included feature",
                Premises_Tag_Category.ServicesOnRequest => "Service on request",
                Premises_Tag_Category.OtherInformation => "Other information",

                _ => enumValue.ToString(),
            };
        }

        public static string Label_Plural(Enum enumValue)
        {
            // We only need to add elements where the enum text isn't right, for example when a space is required or it needs to be named something else
            return enumValue switch
            {
                Premises_Distance_Type.Beach => "Beachs",
                Premises_Distance_Type.Airport => "Airports",
                Premises_Distance_Type.Town => "Towns",
                Premises_Distance_Type.Restaurant => "Restaurants",
                Premises_Distance_Type.Supermarket => "Supermarkets",
                Premises_Distance_Type.GolfCourse => "Golf courses",

                Premises_Room_Type.Bedroom => "Bedrooms",

                Premises_Tag_Category.LivingSpace => "Living spaces",
                Premises_Tag_Category.IndoorFeatures => "Indoor features",
                Premises_Tag_Category.OutdoorFeatures => "Outdoor features",
                Premises_Tag_Category.IncludedFeatures => "Included features",
                Premises_Tag_Category.ServicesOnRequest => "Services on request",
                Premises_Tag_Category.OtherInformation => "Other information",

                _ => Label(enumValue),
            };
        }

        public static string Common_Tag_Icon_Image(Common_Tag_Icon enumValue)
        {
            return enumValue switch
            {
                Common_Tag_Icon.DoubleBed => "icons__def.svg#icon__double-bed",
                Common_Tag_Icon.SingleBed => "icons__def.svg#icon__single-bed",
                Common_Tag_Icon.Kettle => "icons__def.svg#icon__kettle",
                Common_Tag_Icon.Sofa => "icons__def.svg#icon__sofa",
                Common_Tag_Icon.Briefcase => "icons__def.svg#icon__briefcase",
                Common_Tag_Icon.Rocket => "icons__def.svg#icon__rocket",
                Common_Tag_Icon.WashingMachine => "icons__def.svg#icon__washing-machine",
                Common_Tag_Icon.CookingStation => "icons__def.svg#icon__bbq",
                Common_Tag_Icon.TennisRacket => "icons__def.svg#icon__tennis-racket",
                Common_Tag_Icon.Garden => "icons__def.svg#icon__gardens",
                Common_Tag_Icon.Car => "icons__def.svg#icon__car",
                Common_Tag_Icon.Grass => "icons__def.svg#icon__grass",
                Common_Tag_Icon.KnifeFolk => "icons__def.svg#icon__knife-and-fork",
                Common_Tag_Icon.ChefHat => "icons__def.svg#icon__chef-hat",
                Common_Tag_Icon.Hover => "icons__def.svg#icon__hover",
                Common_Tag_Icon.Bell => "icons__def.svg#icon__bell",
                Common_Tag_Icon.Flower => "icons__def.svg#icon__flower",
                Common_Tag_Icon.Pool => "icons__def.svg#icon__pool",
                Common_Tag_Icon.SoapDispenser => "icons__def.svg#icon__soap-dispenser",
                Common_Tag_Icon.FoldedTowels => "icons__def.svg#icon__folded-towels",
                Common_Tag_Icon.HangingTowels => "icons__def.svg#icon__hanging-towel",

                _ => "",
            };
        }
    }
}
