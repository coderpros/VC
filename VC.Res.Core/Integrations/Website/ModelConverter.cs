namespace VC.Res.Core.Integrations.Website
{
    public static class ModelConverter
    {
        public static async Task<Shared.Models.Premise> To_PremiseAsync(Premises.Premise premise, bool bLoadDetailed = true)
        {
            var objReturn = new Shared.Models.Premise();

            if (premise == null) { return objReturn; }
            if (!premise.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = premise.Id;

                if (premise.Website_Id.HasValue) { objReturn.Umb_Id = premise.Website_Id.Value; }
                objReturn.Umb_URL = premise.Website_URL;

                objReturn.Name = premise.Name;
                objReturn.Display_Name = premise.Display_Name;
                objReturn.Overview = premise.Overview;

                objReturn.Address_Line1 = premise.Address_Line1;
                objReturn.Address_Line2 = premise.Address_Line2;
                objReturn.Address_Line3 = premise.Address_Line3;
                objReturn.Address_Town = premise.Address_Town;
                objReturn.Address_Region = premise.Address_Region;
                objReturn.Address_PostCode = premise.Address_PostCode;

                if (premise.Country_Id.HasValue)
                {
                    objReturn.Res_Country = To_Country(await Common.Country.FindAsync(premise.Country_Id.Value));
                }

                if (premise.Region_Id.HasValue)
                {
                    objReturn.Res_Region = To_Region(await Common.Region.FindAsync(premise.Region_Id.Value));
                }

                objReturn.Latitude = premise.Latitude;
                objReturn.Longitude = premise.Longitude;

                objReturn.Guests_Max = premise.Guests_Max;
                objReturn.Guests_Additional = premise.Guests_Additional;
                objReturn.Size = premise.Size;
                objReturn.Rooms_NoBathrooms = premise.Rooms_NoBathrooms;

                objReturn.Website_Pricing_CurrencySymbol = premise.Website_Pricing_CurrencySymbol;
                objReturn.Website_Pricing_CurrencySymbolDisplay = premise.Website_Pricing_CurrencySymbolDisplay;
                objReturn.Website_Pricing_Min = premise.Website_Pricing_Min;
                objReturn.Website_Pricing_Max = premise.Website_Pricing_Max;
                objReturn.Website_Pricing_Type = premise.Website_Pricing_Type;

                if (bLoadDetailed)
                {
                    var lstRooms = Premises.Room.FindAllBy_PremiseAsync(premise.Id);
                    var lstTags = Premises.Tag.FindAllBy_PremiseAsync(premise.Id);
                    var lstDistances = Premises.Distance.FindAllBy_PremiseAsync(premise.Id);
                    var lstRelated = Premises.Related.FindAllBy_PremiseAsync(premise.Id);

                    _ = await lstRooms;
                    foreach (var vRoom in lstRooms.Result)
                    {
                        objReturn.Rooms.Add(To_PremiseRoom(vRoom));
                    }

                    _ = await lstTags;
                    foreach (var vTag in lstTags.Result)
                    {
                        objReturn.Features.Add(To_PremiseFeature(vTag));
                    }

                    _ = await lstDistances;
                    foreach (var vDistance in lstDistances.Result)
                    {
                        objReturn.Distances.Add(To_PremiseDistance(vDistance));
                    }

                    _ = await lstRelated;
                    foreach (var vRelated in lstRelated.Result)
                    {
                        switch (vRelated.Type)
                        {
                            case Enums.Premises_Related_Type.Alternative:
                                objReturn.Alternatives.Add(await To_PremiseAsync(await vRelated.Fetch_PremiseRelatedAsync(), bLoadDetailed: false));
                                break;

                            case Enums.Premises_Related_Type.RentTogether:
                                objReturn.RentedTogether.Add(await To_PremiseAsync(await vRelated.Fetch_PremiseRelatedAsync(), bLoadDetailed: false));
                                break;

                            default: break;
                        }
                    }

                    lstRelated = null;
                    lstDistances = null;
                    lstTags = null;
                    lstRooms = null;
                }

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_PremiseAsync", ex,
                    "premise: " + Newtonsoft.Json.JsonConvert.SerializeObject(premise));
                return objReturn;
            }

            return objReturn;
        }

        public static Shared.Models.PremiseRoom To_PremiseRoom(Premises.Room room)
        {
            var objReturn = new Shared.Models.PremiseRoom();

            if (room == null) { return objReturn; }
            if (!room.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = room.Id;
                objReturn.Type = (int)room.Type;

                objReturn.Name = room.Name;
                objReturn.Description = room.Description;

                objReturn.Beds_Double = room.Beds_Double;
                objReturn.Beds_TwinDouble = room.Beds_TwinDouble;
                objReturn.Beds_Twin = room.Beds_Twin;
                objReturn.Beds_Single = room.Beds_Single;
                objReturn.Beds_Bunk = room.Beds_Bunk;
                objReturn.Beds_Sofa = room.Beds_Sofa;
                objReturn.Beds_Child = room.Beds_Child;
                objReturn.Ensuite = room.Ensuite;

                objReturn.Access_Inside = room.Access_Inside;
                objReturn.Access_Outside = room.Access_Outside;

                objReturn.Order = room.Order;

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_PremiseRoom", ex,
                    "room: " + Newtonsoft.Json.JsonConvert.SerializeObject(room));
                return objReturn;
            }

            return objReturn;
        }

        public static Shared.Models.PremiseFeature To_PremiseFeature(Premises.Tag tag)
        {
            var objReturn = new Shared.Models.PremiseFeature();

            if (tag == null) { return objReturn; }
            if (!tag.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = tag.Id;
                objReturn.Res_Premise_Id = tag.Premise_Id;
                objReturn.Res_Tag_Id = tag.Tag_Id;

                objReturn.Category = (int)tag.Category;

                objReturn.Name = tag.Tag_Name;
                objReturn.Description = tag.Tag_Description;
                objReturn.Icon = tag.Tag_Icon;

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_PremiseFeature", ex,
                    "tag: " + Newtonsoft.Json.JsonConvert.SerializeObject(tag));
                return objReturn;
            }

            return objReturn;
        }

        public static Shared.Models.PremiseDistance To_PremiseDistance(Premises.Distance distance)
        {
            var objReturn = new Shared.Models.PremiseDistance();

            if (distance == null) { return objReturn; }
            if (!distance.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = distance.Id;
                objReturn.Type = (int)distance.Type;

                objReturn.Name = distance.Name;
                objReturn.Description = distance.Description;

                objReturn.KM = distance.KM;
                objReturn.Latitude = distance.Latitude;
                objReturn.Longitude = distance.Longitude;

                objReturn.MinBy_Walk = distance.MinBy_Walk;
                objReturn.MinBy_Drive = distance.MinBy_Drive;
                objReturn.MinBy_Boat = distance.MinBy_Boat;

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_PremiseDistance", ex,
                    "distance: " + Newtonsoft.Json.JsonConvert.SerializeObject(distance));
                return objReturn;
            }

            return objReturn;
        }

        public static Shared.Models.Country To_Country(Common.Country country)
        {
            var objReturn = new Shared.Models.Country();

            if (country == null) { return objReturn; }
            if (!country.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = country.Id;
                objReturn.Name = country.Name;

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_Country", ex,
                    "country: " + Newtonsoft.Json.JsonConvert.SerializeObject(country));
                return objReturn;
            }

            return objReturn;
        }

        public static Shared.Models.Region To_Region(Common.Region region)
        {
            var objReturn = new Shared.Models.Region();

            if (region == null) { return objReturn; }
            if (!region.Loaded) { return objReturn; }

            try
            {
                objReturn.Res_Id = region.Id;
                objReturn.Res_Country_Id = region.Country_Id;
                objReturn.Name = region.Name;

                objReturn.Loaded = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(ModelConverter).ToString(), "To_Region", ex,
                    "region: " + Newtonsoft.Json.JsonConvert.SerializeObject(region));
                return objReturn;
            }

            return objReturn;
        }
    }
}
