using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Premise
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Premises;

        internal static readonly string OrderByDefault = nameof(Name) + " ASC";
        internal static readonly string OrderByDefaultDB = nameof(tblProperty.tblProperty_name) + " ASC";

        public enum FilterOption
        {
            Ids,
            Name,
            Country_Id,
            Country_Ids,
            Region_Id,
            Region_Ids,
            Group_Id,
            Group_Ids,
            Date_Deleted_UTC
        };

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Name { get; set; } = "";

        public string Display_Name { get; set; } = "";

        public int? Website_Id { get; set; } = null;

        public string Website_URL { get; set; } = "";

        public string Overview { get; set; } = "";

        public List<string> OtherWebsiteURLs { get; set; } = new List<string>();

        public Enums.Premises_Premise_Channel Channel { get; set; } = Enums.Premises_Premise_Channel.Unknown;

        public string Address_Line1 { get; set; } = "";
        public string Address_Line2 { get; set; } = "";
        public string Address_Line3 { get; set; } = "";
        public string Address_Town { get; set; } = "";
        public string Address_Region { get; set; } = "";
        public string Address_PostCode { get; set; } = "";
        public int? Country_Id { get; set; } = null;
        public int? Region_Id { get; set; } = null;
        public int NumberOfCollections { get; set; } = 0;

        public decimal? Latitude { get; set; } = null;
        public decimal? Longitude { get; set; } = null;

        public int? Group_Id { get; set; } = null;
        public bool Group_Use_Contacts { get; set; } = false;

        public int Guests_Max { get; set; } = 0;
        public int Guests_Additional { get; set; } = 0;
        public double? Size { get; set; } = null;
        public int Rooms_NoBathrooms { get; set; } = 0;

        public string LicenceNo { get; set; } = "";

        public string Website_Pricing_CurrencySymbol { get; set; } = "";
        public Shared.Enums.Premises_Premise_WebsiteCurrencyDisplay Website_Pricing_CurrencySymbolDisplay { get; set; } = Shared.Enums.Premises_Premise_WebsiteCurrencyDisplay.NotSet;
        public string Website_Pricing_Min { get; set; } = "";
        public string Website_Pricing_Max { get; set; } = "";

        public bool SaveToUmbraco { get; set; } = false;

        public Shared.Enums.Premises_Premise_WebsitePricingType Website_Pricing_Type { get; set; } = Shared.Enums.Premises_Premise_WebsitePricingType.NotSet;

        public DateTime Created_UTC { get; set; } = DateTime.UtcNow;
        public string Created_By { get; set; } = "";

        public DateTime Edited_UTC { get; set; } = DateTime.UtcNow;
        public string Edited_By { get; set; } = "";

        public DateTime? Deleted_UTC { get; set; } = null;
        public string Deleted_By { get; set; } = "";
        public string CreatedUtc { get; set; }
        public string EditedUtc { get; set; }
        public string DeletedUtc { get; set; }
        public string Display_Name_Calculated
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Display_Name)) { return Display_Name; }
                return Name;
            }
        }

        #endregion properties


        #region Constructors

        public Premise() { }

        public Premise(tblProperty efmObject)
        {
            _ = this.Load(efmObject);
        }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblProperties.AsNoTracking().Include(t => t.tblPropertyCollections).FirstOrDefaultAsync(r => r.tblProperty_id == iId);

                if (efmObject != null)
                {
                    bReturn = this.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblProperty efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    var dB = Settings.Config.DBPooledConnection();
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    var efmPropertyCollectionsCount = dB.tblPropertyCollections.AsNoTracking().Count(r => r.tblProperty_id == efmObject.tblProperty_id);
                    
                    this.Id = efmObject.tblProperty_id;

                    this.Name = efmObject.tblProperty_name;
                    this.Display_Name = efmObject.tblProperty_displayName;

                    this.Website_Id = efmObject.tblProperty_websiteId;
                    this.Website_URL = efmObject.tblProperty_websiteURL;

                    this.Overview = efmObject.tblProperty_overview;
                    this.OtherWebsiteURLs = General.ConvertToListString(efmObject.tblProperty_otherWebsiteURLs);
                    this.Channel = (Enums.Premises_Premise_Channel)efmObject.tblProperty_channel;

                    this.Address_Line1 = efmObject.tblProperty_addressLine1;
                    this.Address_Line2 = efmObject.tblProperty_addressLine2;
                    this.Address_Line3 = efmObject.tblProperty_addressLine3;
                    this.Address_Town = efmObject.tblProperty_addressTown;
                    this.Address_Region = efmObject.tblProperty_addressRegion;
                    this.Address_PostCode = efmObject.tblProperty_addressPostCode;

                    this.Country_Id = efmObject.tblCountry_id;
                    this.Region_Id = efmObject.tblRegion_id;

                    this.Group_Id = efmObject.tblPropertyGroup_id;
                    this.Group_Use_Contacts = efmObject.tblProperty_groupUseContacts;
                    if (!this.Group_Id.HasValue) { this.Group_Use_Contacts = false; }

                    this.Longitude = efmObject.tblProperty_long;
                    this.Latitude = efmObject.tblProperty_lat;

                    this.Guests_Max = efmObject.tblProperty_maxGuests;
                    this.Guests_Additional = efmObject.tblProperty_maxGuestsAdditional;
                    this.Size = efmObject.tblProperty_size;
                    this.Rooms_NoBathrooms = efmObject.tblProperty_noBathrooms;
                    this.NumberOfCollections = efmPropertyCollectionsCount;

                    this.LicenceNo = string.IsNullOrWhiteSpace(efmObject.tblProperty_licenceNo) ? "" : efmObject.tblProperty_licenceNo;

                    this.Website_Pricing_CurrencySymbol = string.IsNullOrWhiteSpace(efmObject.tblProperty_webPriceCurrencySymb) ? "" : efmObject.tblProperty_webPriceCurrencySymb;
                    this.Website_Pricing_CurrencySymbolDisplay = (Shared.Enums.Premises_Premise_WebsiteCurrencyDisplay)efmObject.tblProperty_webPriceCurrencyDisplay;
                    this.Website_Pricing_Min = string.IsNullOrWhiteSpace(efmObject.tblProperty_webPriceMin) ? "" : efmObject.tblProperty_webPriceMin;
                    this.Website_Pricing_Max = string.IsNullOrWhiteSpace(efmObject.tblProperty_webPriceMax) ? "" : efmObject.tblProperty_webPriceMax;
                    this.Website_Pricing_Type = (Shared.Enums.Premises_Premise_WebsitePricingType)efmObject.tblProperty_webPriceType;

                    this.SaveToUmbraco = efmObject.tblProperty_saveToUmbraco;

                    this.Created_UTC = efmObject.tblProperty_createdUTC;
                    this.Created_By = efmObject.tblProperty_createdBy;

                    this.Edited_UTC = efmObject.tblProperty_editedUTC;
                    this.Edited_By = efmObject.tblProperty_editedBy;

                    this.Deleted_UTC = efmObject.tblProperty_deletedUTC;
                    this.Deleted_By = efmObject.tblProperty_deletedBy;
                    this.CreatedUtc = efmObject.tblProperty_createdUTC.ToString("dd-mm-yyyy hh:mm:ss") ?? string.Empty;
                    this.EditedUtc = efmObject.tblProperty_editedUTC.ToString("dd-mm-yyyy hh:mm:ss") ?? string.Empty;
                    this.DeletedUtc = efmObject.tblProperty_deletedUTC?.ToString("dd-mm-yyyy hh:mm:ss") ?? string.Empty;

                    this.Loaded = true;
                    
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(this.GetType().ToString(), "Load(tblProperty)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task MaxGuests_Recalculate(int iId)
        {
            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblProperties.FirstOrDefaultAsync(r => r.tblProperty_id == iId);

                if (efmObject != null)
                {
                    // get all the bedrooms
                    var lstBedrooms = await Room.FindAllBy_PremiseAsync(iId, new List<Shared.Enums.Premises_Room_Type> { Shared.Enums.Premises_Room_Type.Bedroom });

                    var iGuests = 0;
                    var iAdditional = 0;

                    foreach (var vBedroom in lstBedrooms)
                    {
                        if (vBedroom.Beds_Double > 0) { iGuests += vBedroom.Beds_Double * 2; }
                        if (vBedroom.Beds_TwinDouble > 0) { iGuests += vBedroom.Beds_TwinDouble * 2; }
                        if (vBedroom.Beds_Twin > 0) { iGuests += vBedroom.Beds_Twin * 2; }
                        if (vBedroom.Beds_Single > 0) { iGuests += vBedroom.Beds_Single; }
                        if (vBedroom.Beds_Bunk > 0) { iGuests += vBedroom.Beds_Bunk * 2; }
                        if (vBedroom.Beds_Sofa > 0) { iAdditional += vBedroom.Beds_Sofa * 2; }
                    }

                    if (efmObject.tblProperty_maxGuests != iGuests || efmObject.tblProperty_maxGuestsAdditional != iAdditional)
                    {
                        efmObject.tblProperty_maxGuests = iGuests;
                        efmObject.tblProperty_maxGuestsAdditional = iAdditional;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            LocalCache.RefreshCache(CacheKey);
                        }
                    }

                    lstBedrooms = null;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "MaxGuests_Recalculate(int)", ex,
                        "iId: " + iId.ToString());
            }
        }

        internal static async Task<bool> Clear_Group(int iGroupId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get a list of the property ids that are going to change
                var lstPremiseIds = await dB.tblProperties.AsNoTracking().Where(r => r.tblPropertyGroup_id == iGroupId).Select(r => r.tblProperty_id).ToListAsync();

                if (lstPremiseIds.Count > 0)
                {
                    if (await dB.tblProperties.Where(r => r.tblPropertyGroup_id == iGroupId).UpdateAsync(r => new tblProperty { tblPropertyGroup_id = null, tblProperty_groupUseContacts = false }) > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);
                    }

                    // update the configs of these properties for inheritance.
                    await Config.Inheritance_RecalculateBy_PremiseAsync(lstPremiseIds);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "Clear_Group(int)", ex,
                    "iGroupId: " + iGroupId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblProperty
                {
                    tblProperty_name = Name.Trim(),
                    tblProperty_displayName = Display_Name.Trim(),

                    tblProperty_websiteId = null,
                    tblProperty_websiteURL = "",

                    tblProperty_overview = Overview,
                    tblProperty_otherWebsiteURLs = General.ConvertToCommaString(OtherWebsiteURLs),
                    tblProperty_channel = (int)Channel,

                    tblProperty_addressLine1 = Address_Line1.Trim(),
                    tblProperty_addressLine2 = Address_Line2.Trim(),
                    tblProperty_addressLine3 = Address_Line3.Trim(),
                    tblProperty_addressTown = Address_Town.Trim(),
                    tblProperty_addressRegion = Address_Region.Trim(),
                    tblProperty_addressPostCode = Address_PostCode.Trim(),

                    tblCountry_id = Country_Id,
                    tblRegion_id = Region_Id,

                    tblProperty_long = Longitude,
                    tblProperty_lat = Latitude,

                    tblPropertyGroup_id = Group_Id,
                    tblProperty_groupUseContacts = Group_Id.HasValue && Group_Use_Contacts,

                    tblProperty_maxGuests = Guests_Max,
                    tblProperty_maxGuestsAdditional = Guests_Additional,
                    tblProperty_size = Size,
                    tblProperty_noBathrooms = Rooms_NoBathrooms,

                    tblProperty_licenceNo = LicenceNo.Trim(),

                    tblProperty_webPriceCurrencySymb = Website_Pricing_CurrencySymbol.Trim(),
                    tblProperty_webPriceCurrencyDisplay = (int)Website_Pricing_CurrencySymbolDisplay,
                    tblProperty_webPriceMin = Website_Pricing_Min.Trim(),
                    tblProperty_webPriceMax = Website_Pricing_Max.Trim(),
                    tblProperty_webPriceType = (int)Website_Pricing_Type,
                    tblProperty_saveToUmbraco = this.SaveToUmbraco,

                    tblProperty_createdUTC = DateTime.UtcNow,
                    tblProperty_createdBy = strBy,

                    tblProperty_editedUTC = DateTime.UtcNow,
                    tblProperty_editedBy = strBy,

                    tblProperty_deletedUTC = null,
                    tblProperty_deletedBy = ""
                };

                _ = dB.tblProperties.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    // create config (the find will automatically create one)
                    _ = await Config.FindBy_PremiseAsync(Id, bUseCache: false);

                    if ((await Integrations.Website.API.Premises_CreateAsync(this)).Result)
                    {
                        _ = await RefreshAsync();
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "CreateAsync(string)", ex,
                        "strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblProperties.FirstOrDefaultAsync(r => r.tblProperty_id == Id);

                if (efmObject != null)
                {
                    var bRequireWebsiteUpdate = false;

                    if (efmObject.tblProperty_name != Name.Trim())
                    {
                        efmObject.tblProperty_name = Name.Trim();
                        bRequireWebsiteUpdate = true;
                    }

                    if (efmObject.tblProperty_displayName != Display_Name.Trim())
                    {
                        efmObject.tblProperty_displayName = Display_Name.Trim();
                        bRequireWebsiteUpdate = true;
                    }

                    efmObject.tblProperty_overview = Overview;
                    efmObject.tblProperty_otherWebsiteURLs = General.ConvertToCommaString(OtherWebsiteURLs);
                    efmObject.tblProperty_channel = (int)Channel;

                    efmObject.tblProperty_addressLine1 = Address_Line1.Trim();
                    efmObject.tblProperty_addressLine2 = Address_Line2.Trim();
                    efmObject.tblProperty_addressLine3 = Address_Line3.Trim();
                    efmObject.tblProperty_addressTown = Address_Town.Trim();
                    efmObject.tblProperty_addressRegion = Address_Region.Trim();
                    efmObject.tblProperty_addressPostCode = Address_PostCode.Trim();

                    efmObject.tblCountry_id = Country_Id;
                    efmObject.tblRegion_id = Region_Id;

                    efmObject.tblProperty_long = Longitude;
                    efmObject.tblProperty_lat = Latitude;

                    var bGroupChange = false;
                    if (efmObject.tblPropertyGroup_id != Group_Id || efmObject.tblProperty_groupUseContacts != Group_Use_Contacts)
                    {
                        bGroupChange = true;
                    }
                    efmObject.tblPropertyGroup_id = Group_Id;
                    efmObject.tblProperty_groupUseContacts = Group_Id.HasValue && Group_Use_Contacts;

                    efmObject.tblProperty_maxGuests = Guests_Max;
                    efmObject.tblProperty_maxGuestsAdditional = Guests_Additional;
                    efmObject.tblProperty_size = Size;
                    efmObject.tblProperty_noBathrooms = Rooms_NoBathrooms;

                    efmObject.tblProperty_licenceNo = LicenceNo.Trim();

                    if (efmObject.tblProperty_webPriceCurrencySymb != Website_Pricing_CurrencySymbol.Trim())
                    {
                        efmObject.tblProperty_webPriceCurrencySymb = Website_Pricing_CurrencySymbol.Trim();
                        bRequireWebsiteUpdate = true;
                    }

                    if (efmObject.tblProperty_webPriceCurrencyDisplay != (int)Website_Pricing_CurrencySymbolDisplay)
                    {
                        efmObject.tblProperty_webPriceCurrencyDisplay = (int)Website_Pricing_CurrencySymbolDisplay;
                        bRequireWebsiteUpdate = true;
                    }

                    if (efmObject.tblProperty_webPriceMin != Website_Pricing_Min.Trim())
                    {
                        efmObject.tblProperty_webPriceMin = Website_Pricing_Min.Trim();
                        bRequireWebsiteUpdate = true;
                    }

                    if (efmObject.tblProperty_webPriceMax != Website_Pricing_Max.Trim())
                    {
                        efmObject.tblProperty_webPriceMax = Website_Pricing_Max.Trim();
                        bRequireWebsiteUpdate = true;
                    }

                    if (efmObject.tblProperty_webPriceType != (int)Website_Pricing_Type)
                    {
                        efmObject.tblProperty_webPriceType = (int)Website_Pricing_Type;
                        bRequireWebsiteUpdate = true;
                    }

                    efmObject.tblProperty_saveToUmbraco = this.SaveToUmbraco;

                    efmObject.tblProperty_editedUTC = DateTime.UtcNow;
                    efmObject.tblProperty_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);

                        if (bGroupChange)
                        {
                            // update config
                            // find the config (incase it doesn't exist)
                            var objConfig = await Config.FindBy_PremiseAsync(Id);

                            _ = await Config.Inheritance_RecalculateAsync(objConfig.Id);
                        }

                        if (bRequireWebsiteUpdate)
                        {
                            if ((await Integrations.Website.API.Premises_UpdateAsync(this)).Result)
                            {
                                _ = await RefreshAsync();
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "SaveAsync(string)", ex,
                    "Id: " + Id.ToString() +
                    ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteAsync(int iId, string strBy, bool bDeleted = true, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var db = Settings.Config.DBConnection();

                var efmObject = await db.tblProperties.FirstOrDefaultAsync(r => r.tblProperty_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblProperty_deletedUTC.HasValue)
                    {
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblProperty_deletedUTC = DateTime.UtcNow;
                            efmObject.tblProperty_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblProperty_deletedUTC = null;
                            efmObject.tblProperty_deletedBy = "";
                        }

                        efmObject.tblProperty_editedBy = strBy;
                        efmObject.tblProperty_editedUTC = DateTime.UtcNow;

                        if (await db.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

                            if (bDeleted)
                            {
                                // deleted from related entries
                                _ = await Related.DeleteFullBy_RelatedAsync(iId, strBy);

                                _ = await Integrations.Website.API.Premises_DeleteAsync(iId);
                            }
                        }
                    }
                }
                else
                {
                    bReturn = false;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        // WE CANNOT DO A FULL DELETE AS THE PROPERTY MIGHT BE USED IN A BOOKING
        //public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true, bool bForce = false)
        //{
        //    var bReturn = false;

        //    try
        //    {
        //        using var dB = Settings.Config.DBConnection();

        //        var efmObject = await dB.tblProperties.FirstOrDefaultAsync(r => r.tblProperty_id == iId);

        //        if (efmObject != null)
        //        {
        //            // can only fully delete if already flagged for deletion
        //            if (efmObject.tblProperty_deletedUTC != null)
        //            {
        //                // can only delete if flagged more than short recycle bin period days ago or this is a force action
        //                if (efmObject.tblProperty_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
        //                {
        //                    var lstTasks = new List<Task>();

        //                    // TODO: Add child elements
        //                    // lstTasks.Add(Season.DeleteFullBy_PremiseAsync(iId, strBy));
        //                    lstTasks.Add(Contact.DeleteFullBy_PremiseAsync(iId, strBy));
        //                    lstTasks.Add(Related.DeleteFullBy_PremiseAsync(iId, strBy));
        //                    lstTasks.Add(Related.DeleteFullBy_RelatedAsync(iId, strBy));
        //                    lstTasks.Add(Distance.DeleteFullBy_PremiseAsync(iId));
        //                    lstTasks.Add(Room.DeleteFullBy_PremiseAsync(iId));
        //                    lstTasks.Add(Tag.DeleteFullBy_PremiseAsync(iId));

        //                    await Task.WhenAll(lstTasks);

        //                    // because the config for the premise is a parent to the season configs, run the delete after the others
        //                    _ = await Config.DeleteFullBy_PremiseAsync(iId, strBy);

        //                    _ = dB.tblProperties.Remove(efmObject);

        //                    if (await dB.SaveChangesAsync() > 0)
        //                    {
        //                        bReturn = true;

        //                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

        //                        _ = await Integrations.Website.API.Premises_DeleteAsync(iId);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            bReturn = true;
        //        }

        //        efmObject = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = Error.Exception(typeof(Premise).ToString(), "DeleteFullAsync(int, string, bool, bool)", ex,
        //            "Id: " + iId.ToString() +
        //            ", strBy: " + strBy.ToString() +
        //            ", bClearCache: " + strBy.ToString() +
        //            ", bForce: " + strBy.ToString());
        //        return bReturn;
        //    }

        //    return bReturn;
        //}

        public static async Task<bool> Update_WebsiteIntegration(int iId, Shared.Models.Premise premise, string strBy)
        {
            var bReturn = false;

            if (premise.Res_Id != iId) { return false; }

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblProperties.FirstOrDefaultAsync(r => r.tblProperty_id == iId);

                if (efmObject != null)
                {
                    efmObject.tblProperty_websiteId = premise.Umb_Id;
                    efmObject.tblProperty_websiteURL = premise.Umb_URL;

                    //// TODO: update to remove update of pricing info from umbraco
                    //efmObject.tblProperty_webPriceCurrencySymb = premise.Website_Pricing_CurrencySymbol.Trim();
                    //efmObject.tblProperty_webPriceCurrencyDisplay = (int)premise.Website_Pricing_CurrencySymbolDisplay;
                    //efmObject.tblProperty_webPriceMin = premise.Website_Pricing_Min.Trim();
                    //efmObject.tblProperty_webPriceMax = premise.Website_Pricing_Max.Trim();
                    //efmObject.tblProperty_webPriceType = (int)premise.Website_Pricing_Type;

                    efmObject.tblProperty_editedUTC = DateTime.UtcNow;
                    efmObject.tblProperty_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "Update_WebsiteIntegration(int, Shared.Models.Premise, string)", ex,
                        "iId: " + iId.ToString() +
                        ", premise: " + Newtonsoft.Json.JsonConvert.SerializeObject(premise) +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public async Task<int> GetPremiseCollectionId(int propertyId, int collectionId)
        {
            var bReturn = 0;
            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = dB.tblPropertyCollections.FirstOrDefault(x => x.tblProperty_id == propertyId && x.tblCollection_id == collectionId);

                if (efmObject != null)
                {
                    bReturn = efmObject.tblPropertyCollection_id;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "GetPremisesCollectionId(int, int)", ex,
                    "propertyId: " + propertyId +
                    ", collectionId: " + collectionId);

                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Premise> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Premise(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Premise();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Premise();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Premise>> FindAllAsync(List<int> iIds, bool bIncDeleted = true)
        {
            var lstReturn = new List<Premise>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in iIds)
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    if (!bIncDeleted)
                    {
                        if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }
                    }

                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Name).ToList();

            return lstReturn;
        }

        public static async Task<List<Premise>> FindAllBy_GroupAsync(int iGroupId, bool bIncDeleted = true)
        {
            return await FindAllBy_GroupAsync(new List<int> { iGroupId }, bIncDeleted);
        }

        public static async Task<List<Premise>> FindAllBy_GroupAsync(List<int> lstGroupIds, bool bIncDeleted = true)
        {
            var lstReturn = new List<Premise>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Premises_IdxGroup) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iGroupId in lstGroupIds)
                {
                    if (lstIndex.TryGetValue(iGroupId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (!bIncDeleted)
                            {
                                if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Name).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Premise>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Premise> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Premises_IdxGroup) is not Dictionary<int, List<int>> dicIndexGroup)
            {
                lstElements = new Dictionary<int, Premise>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Premise> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexGroup = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblProperties.AsNoTracking().ToListAsync())
                    {
                        var obj = new Premise(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to group index
                            if (obj.Group_Id.HasValue)
                            {
                                if (!dicIndexGroup.ContainsKey(obj.Group_Id.Value))
                                {
                                    dicIndexGroup.Add(obj.Group_Id.Value, new List<int>());
                                }

                                dicIndexGroup[obj.Group_Id.Value].Add(obj.Id);
                            }
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Premises_Premises_IdxGroup, dicIndexGroup);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Premise).ToString(), "Global_ListAsync()", ex);
                    return lstElements;
                }
                finally
                {
                    // release lock
                    _ = s_singleCacheBuildLock.Release();
                }
            }

            return lstElements;
        }

        public static async Task<List<Premise>> ListAsync(List<Filter<FilterOption>>? lstFilters = null, bool bClearCache = false)
        {
            return (await ListPagedAsync(lstFilters: lstFilters, iPageSize: 1000000, bClearCache: bClearCache)).Elements;
        }

        public static async Task<PagedData<Premise>> ListPagedAsync(List<Filter<FilterOption>>? lstFilters = null, int iPageSize = 25, int iPage = 1, List<SortOption>? lstOrdering = null, bool bClearCache = false)
        {
            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

            var predicate = CompileWhere(lstFilters);

            var lstElements = (await Global_ListAsync()).Values.AsQueryable().AsExpandable().Where(predicate).ToList();

            var objPagedData = new PagedData<Premise>(lstElements.Count, iPageSize, iPage);

            var strOrderBy = SortOption.CompileOrderBy<Premise>(lstOrdering, OrderByDefault, OrderByDefaultDB, false);

            objPagedData.Elements = lstElements.AsQueryable().OrderBy(strOrderBy).Skip(objPagedData.ElementsToSkip).Take(objPagedData.ElementsToTake).ToList();
            objPagedData.ExportElements = lstElements.AsQueryable().OrderBy(strOrderBy).ToList();

            return objPagedData;
        }

        private static Expression<Func<Premise, bool>> CompileWhere(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<Premise>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<Premise>(false); }

            if (lstFilters == null) { return predicate; }

            try
            {
                foreach (var vFilter in lstFilters)
                {
                    switch (vFilter.Option)
                    {
                        case FilterOption.Ids:
                            {
                                if (vFilter.Exclude)
                                {
                                    var lstIds = vFilter.Value_ListInt();
                                    predicate = predicate.And(r => !lstIds.Contains(r.Id));
                                }
                                else
                                {
                                    var lstIds = vFilter.Value_ListInt();
                                    predicate = predicate.And(r => lstIds.Contains(r.Id));
                                }
                            }
                            break;

                        case FilterOption.Name:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name.ToLower().Contains(strSearch) || r.Display_Name.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Country_Id:
                            {
                                var i = vFilter.Value_IntNullable();
                                predicate = predicate.And(r => r.Country_Id == i);
                            }
                            break;
                        case FilterOption.Country_Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
#pragma warning disable CS8629 // Nullable value type may be null.
                                predicate = predicate.And(r => r.Country_Id != null).And(r => lstIds.Contains(r.Country_Id.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
                            }
                            break;

                        case FilterOption.Region_Id:
                            {
                                var i = vFilter.Value_IntNullable();
                                predicate = predicate.And(r => r.Region_Id == i);
                            }
                            break;
                        case FilterOption.Region_Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
#pragma warning disable CS8629 // Nullable value type may be null.
                                predicate = predicate.And(r => r.Region_Id != null).And(r => lstIds.Contains(r.Region_Id.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
                            }
                            break;

                        case FilterOption.Group_Id:
                            {
                                var i = vFilter.Value_IntNullable();
                                predicate = predicate.And(r => r.Group_Id == i);
                            }
                            break;
                        case FilterOption.Group_Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
#pragma warning disable CS8629 // Nullable value type may be null.
                                predicate = predicate.And(r => r.Group_Id != null).And(r => lstIds.Contains(r.Group_Id.Value));
#pragma warning restore CS8629 // Nullable value type may be null.
                            }
                            break;

                        case FilterOption.Date_Deleted_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC == dfDates.Equal.Value);
                                }

                                if (dfDates.EqualNull.HasValue)
                                {
                                    if (dfDates.EqualNull.Value)
                                    {
                                        predicate = predicate.And(r => r.Deleted_UTC == null);
                                    }
                                    else
                                    {
                                        predicate = predicate.And(r => r.Deleted_UTC != null);
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "CompileWhere(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }

        internal static string OrderByConvert(string strFieldName, bool bDBVersion)
        {
            try
            {
                var strObject = "";
                var strDB = "";

                switch (strFieldName)
                {
                    case nameof(Name):
                        strObject = nameof(Name);
                        strDB = nameof(tblProperty.tblProperty_name);
                        break;

                    case nameof(Created_UTC):
                        strObject = nameof(Created_UTC);
                        strDB = nameof(tblProperty.tblProperty_createdUTC);
                        break;
                    case nameof(Edited_UTC):
                        strObject = nameof(Edited_UTC);
                        strDB = nameof(tblProperty.tblProperty_editedUTC);
                        break;
                    default: return "";
                }

                if (bDBVersion)
                {
                    return strDB;
                }
                else
                {
                    return strObject;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "OrderByConvert(string, bool)", ex,
                    "strFieldName: " + strFieldName.ToString() +
                    ", bDBVersion: " + bDBVersion);
                return "";
            }
        }

        public static DataTable Export(List<Premise> args, List<Core.Common.Country> country)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Utilities.General.CreateColumn("Id", "ID", typeof(int)));
            dt.Columns.Add(Utilities.General.CreateColumn("Name", "Name", typeof(string)));
            dt.Columns.Add(Utilities.General.CreateColumn("DisplayName", "Display Name", typeof(string)));
            dt.Columns.Add(Utilities.General.CreateColumn("Country", "Country", typeof(string)));
            DataTable data = General.ToTable(args);
            string[] columns = data.Columns.OfType<DataColumn>()
    .Where(c => c.ColumnName != "Loaded" && c.ColumnName != "Display_Name_Calculated")
    .Select(c => c.ColumnName).ToArray();
            dt = new DataView(data).ToTable(false, columns);

            //if (args != null && args.Count > 0)
            //{

            //    foreach (var item in args)
            //    {
            //        var dr = dt.NewRow();
            //        dr["Id"] = item.Id;
            //        dr["Name"] = item.Name;
            //        dr["DisplayName"] = item.Display_Name;
            //        if (country != null && country.Count > 0 && item.Country_Id.HasValue)
            //            dr["Country"] = country.FirstOrDefault(x => x.Id == item.Country_Id.Value)!.Name;
            //        else
            //            dr["Country"] = "";
            //        dt.Rows.Add(dr);
            //        dr = null;
            //    }
            //}
            return dt ?? new DataTable();
        }

        #endregion lists
    }
}
