using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;


namespace VC.Res.Core.Premises
{
    public class Contact : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Contacts;

        #region Properties

        public int? Premise_Id { get; private set; } = null;

        public int? Group_Id { get; private set; } = null;

        public int Contact_Id { get; private set; } = 0;

        public bool Config_Primary { get; private set; } = false;

        public List<Enums.Contacts_Contact_Category> Categories { get; set; } = new List<Enums.Contacts_Contact_Category>();

        // PAPM = Portal Access - Property Management
        /// <summary>
        /// Access to general property information for review/completion
        /// </summary>
        public bool PAPM_Info { get; set; } = false;

        /// <summary>
        /// Access to the property rates
        /// </summary>
        public bool PAPM_Rates { get; set; } = false;

        /// <summary>
        /// Access to the property availability
        /// </summary>
        public bool PAPM_Availability { get; set; } = false;

        /// <summary>
        /// Access to bookings at the property
        /// </summary>
        public bool PAPM_Bookings { get; set; } = false;

        /// <summary>
        /// Access to provide authorisation/confirmation of a booking at the property
        /// </summary>
        public bool PAPM_Booking_Confirmation { get; set; } = false;

        /// <summary>
        /// Access to remit slips issued to property
        /// </summary>
        public bool PAPM_RemitSlip { get; set; } = false;

        // Notifications
        /// <summary>
        /// Any notifications requesting property information to be reviewing/completing
        /// </summary>
        public bool Notifi_Info { get; set; } = false;

        /// <summary>
        /// Any notifications requesting property rates to be reviewed
        /// </summary>
        public bool Notifi_Rates { get; set; } = false;

        /// <summary>
        /// Any notifications requesting property availability to be reviewed
        /// </summary>
        public bool Notifi_Availability { get; set; } = false;

        /// <summary>
        /// Any notifications regarding bookings made at the property, including confirmation of a booking
        /// </summary>
        public bool Notifi_Bookings { get; set; } = false;

        /// <summary>
        /// Notifications requesting a booking to be confirmed
        /// </summary>
        public bool Notifi_Booking_Confirmation { get; set; } = false;

        /// <summary>
        /// Any notifications regarding Remit slips for the property
        /// </summary>
        public bool Notifi_RemitSlip { get; set; } = false;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Contact() { }

        private Contact(tblPropertyContact efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyContacts.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyContact_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyContact efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyContact_id;

                    Premise_Id = efmObject.tblProperty_id;
                    Group_Id = efmObject.tblPropertyGroup_id;
                    Contact_Id = efmObject.tblContact_id;

                    Config_Primary = efmObject.tblPropertyContact_configPrimary;
                    Categories = General.ConvertToListEnums<Enums.Contacts_Contact_Category>(efmObject.tblPropertyContact_categories);

                    PAPM_Info = efmObject.tblPropertyContact_papmInfo;
                    PAPM_Availability = efmObject.tblPropertyContact_papmAvailability;
                    PAPM_Bookings = efmObject.tblPropertyContact_papmBookings;
                    PAPM_Booking_Confirmation = efmObject.tblPropertyContact_papmBookingConfirmation;
                    PAPM_Rates = efmObject.tblPropertyContact_papmRates;
                    PAPM_RemitSlip = efmObject.tblPropertyContact_papmRemitSlip;

                    Notifi_Info = efmObject.tblPropertyContact_notifiInfo;
                    Notifi_Rates = efmObject.tblPropertyContact_notifiRates;
                    Notifi_Availability = efmObject.tblPropertyContact_notifiAvailability;
                    Notifi_Bookings = efmObject.tblPropertyContact_notifiBookings;
                    Notifi_Booking_Confirmation = efmObject.tblPropertyContact_notifiBookingConfirmation;
                    Notifi_RemitSlip = efmObject.tblPropertyContact_notifiRemitSlip;

                    Created_UTC = efmObject.tblPropertyContact_createdUTC;
                    Created_By = efmObject.tblPropertyContact_createdBy;

                    Edited_UTC = efmObject.tblPropertyContact_editedUTC;
                    Edited_By = efmObject.tblPropertyContact_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyContact)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        /// <summary>
        /// Give a premise contact id, calculates the premises that would be effected
        /// by a change to it (i.e the contact is assigned to the premise or a group used by it)
        /// </summary>
        /// <param name="lstPremiseContactIds">Premise Contact Ids to check</param>
        /// <returns>List of premise ids that are effected by the given premise contact</returns>
        private static async Task<List<int>> AffectedPremises(List<int> lstPremiseContactIds)
        {
            var lstReturn = new List<int>();

            if (lstPremiseContactIds.Count < 1) { return lstReturn; }

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var lstPremiseContacts = await dB.tblPropertyContacts.AsNoTracking().Where(r => lstPremiseContactIds.Contains(r.tblPropertyContact_id)).ToListAsync();

                // create list of premise ids
#pragma warning disable CS8629 // Nullable value type may be null.
                lstReturn = lstPremiseContacts.Where(r => r.tblProperty_id != null).Select(r => r.tblProperty_id.Value).ToList();
#pragma warning restore CS8629 // Nullable value type may be null.

                // get a list of the group ids then find properties using the group
#pragma warning disable CS8629 // Nullable value type may be null.
                var lstGroupIds = lstPremiseContacts.Where(r => r.tblPropertyGroup_id != null).Select(r => r.tblPropertyGroup_id.Value).ToList();
#pragma warning restore CS8629 // Nullable value type may be null.

                if (lstGroupIds.Count > 0)
                {
                    var lstFilters_Premise = new FilterList<Premise.FilterOption>();
                    lstFilters_Premise.Add(Premise.FilterOption.Group_Ids, lstGroupIds);

                    lstReturn = lstReturn.Concat((await Premise.ListAsync(lstFilters_Premise.Filters)).Select(r => r.Id).ToList()).ToList();
                }

                lstGroupIds = null;
                lstPremiseContacts = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "AffectedPremises(List<int>)", ex);
                return lstReturn;
            }

            return lstReturn;
        }


        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_PremiseAsync(int iPremiseId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get the premise contact ids to be affected
                var lstPremiseContactIds = await dB.tblPropertyContacts.AsNoTracking().Where(r => r.tblProperty_id == iPremiseId).Select(r => r.tblPropertyContact_id).ToListAsync();

                if (lstPremiseContactIds.Count > 0)
                {
                    // get premises that will be affected to recalc inheritance
                    var lstPremiseIds = await AffectedPremises(lstPremiseContactIds);

                    // delete the rows
                    if (await dB.tblPropertyContacts.Where(r => r.tblProperty_id == iPremiseId).DeleteAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);

                        await Config.Inheritance_RecalculateBy_PremiseAsync(lstPremiseIds);
                    }

                    lstPremiseIds = null;
                }

                lstPremiseContactIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "DeleteFullBy_PremiseAsync(int, string)", ex,
                    "iPremiseId: " + iPremiseId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> DeleteFullBy_GroupAsync(int iGroupId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get the premise contact ids to be affected
                var lstPremiseContactIds = await dB.tblPropertyContacts.AsNoTracking().Where(r => r.tblPropertyGroup_id == iGroupId).Select(r => r.tblPropertyContact_id).ToListAsync();

                if (lstPremiseContactIds.Count > 0)
                {
                    // get premises that will be affected to recalc inheritance
                    var lstPremiseIds = await AffectedPremises(lstPremiseContactIds);

                    if (await dB.tblPropertyContacts.Where(r => r.tblPropertyGroup_id == iGroupId).DeleteAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);

                        await Config.Inheritance_RecalculateBy_PremiseAsync(lstPremiseIds);
                    }

                    lstPremiseIds = null;
                }

                lstPremiseContactIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "DeleteFullBy_GroupAsync(int, string)", ex,
                    "iGroupId: " + iGroupId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> DeleteFullBy_ContactAsync(int iContactId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get the premise contact ids to be affected
                var lstPremiseContactIds = await dB.tblPropertyContacts.AsNoTracking().Where(r => r.tblContact_id == iContactId).Select(r => r.tblPropertyContact_id).ToListAsync();

                if (lstPremiseContactIds.Count > 0)
                {
                    // get premises that will be affected to recalc inheritance
                    var lstPremiseIds = await AffectedPremises(lstPremiseContactIds);

                    if (await dB.tblPropertyContacts.Where(r => r.tblContact_id == iContactId).DeleteAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);

                        await Config.Inheritance_RecalculateBy_PremiseAsync(lstPremiseIds);
                    }

                    lstPremiseIds = null;
                }

                lstPremiseContactIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "DeleteFullBy_ContactAsync(int, string)", ex,
                    "iContactId: " + iContactId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int? iPremiseId, int? iGroupId, int iContactId, string strBy)
        {
            if (iPremiseId.HasValue && iGroupId.HasValue) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // if already exists, return current
                var efmObject = await dB.tblPropertyContacts.FirstOrDefaultAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyGroup_id == iGroupId && r.tblContact_id == iContactId);

                if (efmObject != null)
                {
                    // already exists
                    bReturn = Load(efmObject);
                }
                else
                {
                    efmObject = new tblPropertyContact
                    {
                        tblProperty_id = iPremiseId,
                        tblPropertyGroup_id = iGroupId,
                        tblContact_id = iContactId,

                        tblPropertyContact_configPrimary = !await dB.tblPropertyContacts.AsNoTracking().AnyAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyGroup_id == iGroupId),

                        tblPropertyContact_categories = General.ConvertToCommaString(Categories),

                        tblPropertyContact_papmAvailability = PAPM_Availability,
                        tblPropertyContact_papmBookingConfirmation = PAPM_Booking_Confirmation,
                        tblPropertyContact_papmBookings = PAPM_Bookings,
                        tblPropertyContact_papmInfo = PAPM_Info,
                        tblPropertyContact_papmRates = PAPM_Rates,
                        tblPropertyContact_papmRemitSlip = PAPM_RemitSlip,

                        tblPropertyContact_notifiAvailability = Notifi_Availability,
                        tblPropertyContact_notifiBookingConfirmation = Notifi_Booking_Confirmation,
                        tblPropertyContact_notifiBookings = Notifi_Bookings,
                        tblPropertyContact_notifiInfo = Notifi_Info,
                        tblPropertyContact_notifiRates = Notifi_Rates,
                        tblPropertyContact_notifiRemitSlip = Notifi_RemitSlip,

                        tblPropertyContact_createdUTC = DateTime.UtcNow,
                        tblPropertyContact_createdBy = strBy,

                        tblPropertyContact_editedUTC = DateTime.UtcNow,
                        tblPropertyContact_editedBy = strBy,
                    };

                    _ = dB.tblPropertyContacts.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);

                        if (Config_Primary)
                        {
                            // check the contact has a config (it will auto create one if not found)
                            var objContactConfig = await Config.FindBy_ContactAsync(iContactId);

                            await Config.Inheritance_RecalculateBy_PremiseAsync(await AffectedPremises(new List<int> { Id }));
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "CreateAsync(int?, int?, int, string)", ex,
                    "iPremiseId: " + iPremiseId.ToString() +
                    "iGroupId: " + iGroupId.ToString() +
                    "iContactId: " + iContactId.ToString() +
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

                var efmObject = await dB.tblPropertyContacts.FirstOrDefaultAsync(r => r.tblPropertyContact_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyContact_categories = General.ConvertToCommaString(Categories);

                    efmObject.tblPropertyContact_papmAvailability = PAPM_Availability;
                    efmObject.tblPropertyContact_papmBookingConfirmation = PAPM_Booking_Confirmation;
                    efmObject.tblPropertyContact_papmBookings = PAPM_Bookings;
                    efmObject.tblPropertyContact_papmInfo = PAPM_Info;
                    efmObject.tblPropertyContact_papmRates = PAPM_Rates;
                    efmObject.tblPropertyContact_papmRemitSlip = PAPM_RemitSlip;

                    efmObject.tblPropertyContact_notifiAvailability = Notifi_Availability;
                    efmObject.tblPropertyContact_notifiBookingConfirmation = Notifi_Booking_Confirmation;
                    efmObject.tblPropertyContact_notifiBookings = Notifi_Bookings;
                    efmObject.tblPropertyContact_notifiInfo = Notifi_Info;
                    efmObject.tblPropertyContact_notifiRates = Notifi_Rates;
                    efmObject.tblPropertyContact_notifiRemitSlip = Notifi_RemitSlip;

                    efmObject.tblPropertyContact_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyContact_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "SaveAsync(string)", ex,
                    "Id: " + Id.ToString() +
                    ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyContacts.FirstOrDefaultAsync(r => r.tblPropertyContact_id == iId);

                if (efmObject != null)
                {
                    // track if primary config, if it is we will need to update premise config inheritances
                    var bConfigPrimary = efmObject.tblPropertyContact_configPrimary;
                    var lstPremiseIds = new List<int>();

                    if (bConfigPrimary)
                    {
                        // create a list of premises this contact effects and update their configs
                        lstPremiseIds = await AffectedPremises(new List<int> { iId });
                    }

                    _ = dB.tblPropertyContacts.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

                        if (bConfigPrimary)
                        {
                            // process configs/properties to update
                            await Config.Inheritance_RecalculateBy_PremiseAsync(lstPremiseIds);
                        }
                    }

                    lstPremiseIds = null;
                }
                else
                {
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Set_PrimaryAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyContacts.FirstOrDefaultAsync(r => r.tblPropertyContact_id == iId);

                if (efmObject != null)
                {
                    if (efmObject.tblPropertyContact_configPrimary)
                    {
                        bReturn = true;
                    }
                    else
                    {
                        // clear primary from any other entry assigned to this contact
                        _ = await dB.tblPropertyContacts.Where(r => r.tblProperty_id == efmObject.tblProperty_id && r.tblPropertyGroup_id == efmObject.tblPropertyGroup_id)
                                                        .UpdateAsync(r => new tblPropertyContact { tblPropertyContact_configPrimary = false });

                        efmObject.tblPropertyContact_configPrimary = true;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            LocalCache.RefreshCache(CacheKey);

                            // check the contact has a config (it will auto create one if not found)
                            var objContactConfig = await Config.FindBy_ContactAsync(efmObject.tblContact_id);

                            // update configs of premises
                            await Config.Inheritance_RecalculateBy_PremiseAsync(await AffectedPremises(new List<int> { iId }));
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "Set_PrimaryAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Contact> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Contact(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Contact();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Contact();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Contact>> FindAllAsync(List<int> lstIds)
        {
            var lstReturn = new List<Contact>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in lstIds.Distinct())
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Contact>> FindAllBy_GroupAsync(int iGroupId)
        {
            return await FindAllBy_GroupAsync(new List<int> { iGroupId });
        }

        public static async Task<List<Contact>> FindAllBy_GroupAsync(List<int> lstGroupIds)
        {
            var lstReturn = new List<Contact>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxPremiseGroup) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iGroupId in lstGroupIds.Distinct())
                {
                    if (lstIndex.TryGetValue(iGroupId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Contact>> FindAllBy_PremiseAsync(int iPremiseId)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId });
        }

        public static async Task<List<Contact>> FindAllBy_PremiseAsync(List<int> lstPremiseIds)
        {
            var lstReturn = new List<Contact>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPremiseId in lstPremiseIds.Distinct())
                {
                    if (lstIndex.TryGetValue(iPremiseId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Contact>> FindAllBy_ContactAsync(int iContactId)
        {
            return await FindAllBy_ContactAsync(new List<int> { iContactId });
        }

        public static async Task<List<Contact>> FindAllBy_ContactAsync(List<int> lstContactId)
        {
            var lstReturn = new List<Contact>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxContact) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iContactId in lstContactId.Distinct())
                {
                    if (lstIndex.TryGetValue(iContactId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        #endregion finders


        #region Lists
        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Contact>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Contact> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxContact) is not Dictionary<int, List<int>> dicIndexContact ||
                LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Contacts_IdxPremiseGroup) is not Dictionary<int, List<int>> dicIndexPremiseGroup)
            {
                lstElements = new Dictionary<int, Contact>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Contact> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexContact = new Dictionary<int, List<int>>();
                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicIndexPremiseGroup = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblPropertyContacts.AsNoTracking().ToListAsync())
                    {
                        var obj = new Contact(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to contact index
                            if (!dicIndexContact.ContainsKey(obj.Contact_Id))
                            {
                                dicIndexContact.Add(obj.Contact_Id, new List<int>());
                            }

                            dicIndexContact[obj.Contact_Id].Add(obj.Id);

                            // add to premise index
                            if (obj.Premise_Id.HasValue)
                            {
                                if (!dicIndexPremise.ContainsKey(obj.Premise_Id.Value))
                                {
                                    dicIndexPremise.Add(obj.Premise_Id.Value, new List<int>());
                                }
                                dicIndexPremise[obj.Premise_Id.Value].Add(obj.Id);
                            }

                            // add to group index
                            if (obj.Group_Id.HasValue)
                            {
                                if (!dicIndexPremiseGroup.ContainsKey(obj.Group_Id.Value))
                                {
                                    dicIndexPremiseGroup.Add(obj.Group_Id.Value, new List<int>());
                                }
                                dicIndexPremiseGroup[obj.Group_Id.Value].Add(obj.Id);
                            }
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Premises_Contacts_IdxPremiseGroup, dicIndexPremiseGroup);
                    LocalCache.Set(LocalCache.Key.Premises_Contacts_IdxContact, dicIndexContact);
                    LocalCache.Set(LocalCache.Key.Premises_Contacts_IdxPremise, dicIndexPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Contact).ToString(), "Global_ListAsync()", ex);
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

        #endregion lists
    }
}
