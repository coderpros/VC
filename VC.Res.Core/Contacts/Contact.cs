using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Security.Cryptography;
using Azure.Core;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VC.Res.Core.Database;
using VC.Res.Core.Integrations.Zoho;
using VC.Res.Core.Models;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Contacts
{
    public class Contact
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Contacts_Contacts;

        internal static readonly string OrderByDefault = nameof(Name_Last) + " ASC, " + nameof(Name_First) + " ASC";
        internal static readonly string OrderByDefaultDB = nameof(tblContact.tblContact_lastName) + " ASC, " + nameof(tblContact.tblContact_firstName) + " ASC";

        public enum FilterOption
        {
            Ids,
            Name_First,
            Name_Last,
            Name_Full,
            Company,
            Category,
            Categories,
            Date_Deleted_UTC
        };

        #region Properties

        public string Zoho_Api_Url { get; set; }
        public string Zoho_Refresh_Token { get; set; }

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Zoho_Id { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string Title { get; set; } = "";

        public string Name_First { get; set; } = "";

        public string Name_Middle { get; set; } = "";

        public string Name_Last { get; set; } = "";

        public string Name_Full
        {
            get
            {
                var strReturn = Name_First;

                if (!string.IsNullOrWhiteSpace(strReturn) && !string.IsNullOrWhiteSpace(Name_Last))
                {
                    strReturn += " ";
                }

                strReturn += Name_Last;

                return strReturn;
            }
        }

        public string WebsiteURL { get; set; } = "";

        public Enums.Contacts_Contact_PreferredContactMethod PreferredContactMethod { get; set; } = Enums.Contacts_Contact_PreferredContactMethod.Unknown;

        public List<Enums.Contacts_Contact_Category> Categories { get; set; } = new List<Enums.Contacts_Contact_Category>();

        public string Note { get; set; } = "";

        public Enums.Shared_NumericValueType Agent_AmountType { get; set; } = Enums.Shared_NumericValueType.Unknown;

        public decimal? Agent_Amount { get; set; } = null;

        public Enums.Shared_AgentPaymentPoint Agent_PaymentPoint { get; set; } = Enums.Shared_AgentPaymentPoint.Unknown;

        public decimal? Agent_PaymentDeposit { get; set; } = null;

        public decimal? Agent_PaymentInterim { get; set; } = null;

        public decimal? Agent_PaymentBalance { get; set; } = null;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;

        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Contact()
        {
            
        }

        private Contact(tblContact efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblContacts.AsNoTracking().FirstOrDefaultAsync(r => r.tblContact_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }
                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblContact efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblContact_id;
                    Zoho_Id = efmObject.tblContact_zohoId;

                    CompanyName = efmObject.tblContact_companyName;
                    Title = efmObject.tblContact_title;
                    Name_First = efmObject.tblContact_firstName;
                    Name_Middle = efmObject.tblContact_middleName;
                    Name_Last = efmObject.tblContact_lastName;

                    WebsiteURL = efmObject.tblContact_websiteURL;

                    PreferredContactMethod = (Enums.Contacts_Contact_PreferredContactMethod)efmObject.tblContact_prefContactMethod;
                    Categories = General.ConvertToListEnums<Enums.Contacts_Contact_Category>(efmObject.tblContact_categories);
                    Note = efmObject.tblContact_note;

                    Agent_AmountType = (Enums.Shared_NumericValueType)efmObject.tblContact_agentAmountType;
                    Agent_Amount = efmObject.tblContact_agentAmount;
                    Agent_PaymentPoint = (Enums.Shared_AgentPaymentPoint)efmObject.tblContact_agentPaymentPoint;
                    Agent_PaymentDeposit = efmObject.tblContact_agentPaymentDeposit;
                    Agent_PaymentInterim = efmObject.tblContact_agentPaymentInterim;
                    Agent_PaymentBalance = efmObject.tblContact_agentPaymentBalance;

                    Created_UTC = efmObject.tblContact_createdUTC;
                    Created_By = efmObject.tblContact_createdBy;

                    Edited_UTC = efmObject.tblContact_editedUTC;
                    Edited_By = efmObject.tblContact_editedBy;

                    Deleted_UTC = efmObject.tblContact_deletedUTC;
                    Deleted_By = efmObject.tblContact_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblContact)", ex);
                return false;
            }

            return bReturn;
        }


        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();
                var efmObject = new tblContact
                {


                    tblContact_zohoId = Zoho_Id,

                    tblContact_companyName = CompanyName,

                    tblContact_title = Title,
                    tblContact_firstName = Name_First,
                    tblContact_middleName = Name_Middle,
                    tblContact_lastName = Name_Last,

                    tblContact_websiteURL = WebsiteURL,

                    tblContact_prefContactMethod = (int)PreferredContactMethod,
                    tblContact_categories = General.ConvertToCommaString(Categories),
                    tblContact_note = Note,

                    tblContact_agentAmountType = (int)Agent_AmountType,
                    tblContact_agentAmount = Agent_Amount,
                    tblContact_agentPaymentPoint = (int)Agent_PaymentPoint,
                    tblContact_agentPaymentDeposit = Agent_PaymentDeposit,
                    tblContact_agentPaymentInterim = Agent_PaymentInterim,
                    tblContact_agentPaymentBalance = Agent_PaymentBalance,

                    tblContact_createdUTC = DateTime.UtcNow,
                    tblContact_createdBy = strBy,

                    tblContact_editedUTC = DateTime.UtcNow,
                    tblContact_editedBy = strBy,

                    tblContact_deletedUTC = null,
                    tblContact_deletedBy = ""
                };

                _ = dB.tblContacts.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);
                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Contact).ToString(), "CreateAsync(string)", ex,
                        "strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        #region zoho contact api update
        public async Task<string> CreateOrUpdateZohoContactAsync(List<ZohoContactRequest> args, int contactId)
        {
            using var dB = Settings.Config.DBPooledConnection();
            if (!string.IsNullOrEmpty(args.FirstOrDefault()?.id))
            {
                var emaiObj = dB.tblContactEmails.FirstOrDefault(x => x.tblContact_id == contactId) ?? new tblContactEmail();
                var mobObj = dB.tblContactTels.FirstOrDefault(x => x.tblContact_id == contactId);
                args.ForEach(x => {
                    x.Phone = string.Format("{0}{1}", mobObj?.tblContactTel_countryCode, mobObj?.tblContactTel_no);
                    x.Email = emaiObj?.tblContactEmail_address ?? "";
                });
            }
            ContactRequest data = new ContactRequest(args);
            ContactResponse res = await new ZConfiguration().ZClientRequest<ContactResponse>(string.IsNullOrEmpty(args.FirstOrDefault()?.id) ? "POST" : "PUT", data);

            if (res != null && res.data != null)
            {
                string zohoId = res.data.Select(x => x.details?.id).FirstOrDefault() ?? "";
                if (!string.IsNullOrEmpty(zohoId))
                {
                    var updateContact = dB.tblContacts.Where(x => x.tblContact_id == contactId).FirstOrDefault() ?? new tblContact();
                    updateContact.tblContact_zohoId = zohoId;
                    dB.SaveChanges();
                }
                return zohoId ?? "";
            }
            else
            {
                return "";
            }
        }
        #endregion

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContacts.FirstOrDefaultAsync(r => r.tblContact_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblContact_zohoId = Zoho_Id;

                    efmObject.tblContact_companyName = CompanyName;
                    efmObject.tblContact_title = Title;
                    efmObject.tblContact_firstName = Name_First;
                    efmObject.tblContact_middleName = Name_Middle;
                    efmObject.tblContact_lastName = Name_Last;

                    efmObject.tblContact_websiteURL = WebsiteURL;

                    efmObject.tblContact_prefContactMethod = (int)PreferredContactMethod;
                    efmObject.tblContact_categories = General.ConvertToCommaString(Categories);
                    efmObject.tblContact_note = Note;

                    efmObject.tblContact_agentAmountType = (int)Agent_AmountType;
                    efmObject.tblContact_agentAmount = Agent_Amount;
                    efmObject.tblContact_agentPaymentPoint = (int)Agent_PaymentPoint;
                    efmObject.tblContact_agentPaymentDeposit = Agent_PaymentDeposit;
                    efmObject.tblContact_agentPaymentInterim = Agent_PaymentInterim;
                    efmObject.tblContact_agentPaymentBalance = Agent_PaymentBalance;

                    efmObject.tblContact_editedUTC = DateTime.UtcNow;
                    efmObject.tblContact_editedBy = strBy;

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
                _ = Error.Exception(typeof(Contact).ToString(), "Save(string)", ex,
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

                var efmObject = await db.tblContacts.FirstOrDefaultAsync(r => r.tblContact_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblContact_deletedUTC.HasValue)
                    {
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblContact_deletedUTC = DateTime.UtcNow;
                            efmObject.tblContact_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblContact_deletedUTC = null;
                            efmObject.tblContact_deletedBy = "";
                        }

                        efmObject.tblContact_editedBy = strBy;
                        efmObject.tblContact_editedUTC = DateTime.UtcNow;

                        if (await db.SaveChangesAsync() > 0)
                        {

                            bReturn = true;
                            string zohoId = efmObject.tblContact_zohoId;
                            if (!string.IsNullOrEmpty(zohoId))
                                await new ZConfiguration().ZClientRequest<bool>("DELETE", new { }, $@"Contacts/{zohoId}");
                            // delete the contact from any premises or premise groups
                            _ = await Premises.Contact.DeleteFullBy_ContactAsync(iId, strBy);

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
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
                _ = Error.Exception(typeof(Contact).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        // WE CANNOT DO A FULL DELETE AS THE CONTACT MIGHT BE USED IN A BOOKING
        //public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true, bool bForce = false)
        //{
        //    var bReturn = false;

        //    try
        //    {
        //        using var dB = Settings.Config.DBConnection();

        //        var efmObject = await dB.tblContacts.FirstOrDefaultAsync(r => r.tblContact_id == iId);

        //        if (efmObject != null)
        //        {
        //            // can only fully delete if already flagged for deletion
        //            if (efmObject.tblContact_deletedUTC != null)
        //            {
        //                // can only delete if flagged more than short recycle bin period days ago or this is a force action
        //                if (efmObject.tblContact_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
        //                {
        //                    var lstTasks = new List<Task>();

        //                    //add tasks
        //                    lstTasks.Add(Premises.Contact.DeleteFullBy_ContactAsync(iId, strBy));
        //                    lstTasks.Add(Address.DeleteFullBy_ContactAsync(iId));
        //                    lstTasks.Add(Email.DeleteFullBy_ContactAsync(iId));
        //                    lstTasks.Add(Tag.DeleteFullBy_ContactAsync(iId));
        //                    lstTasks.Add(TelephoneNo.DeleteFullBy_ContactAsync(iId));

        //                    await Task.WhenAll(lstTasks);

        //                    _ = await Premises.Config.DeleteFullBy_ContactAsync(iId, strBy);

        //                    _ = dB.tblContacts.Remove(efmObject);

        //                    if (await dB.SaveChangesAsync() > 0)
        //                    {
        //                        bReturn = true;

        //                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
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
        //        _ = Error.Exception(typeof(Contact).ToString(), "DeleteFullAsync(int, string, bool, bool)", ex,
        //            "iId: " + iId.ToString() +
        //            ", strBy: " + strBy.ToString() +
        //            ", bClearCache: " + bClearCache.ToString() +
        //            ", bForce: " + strBy.ToString());
        //        return bReturn;
        //    }

        //    return bReturn;
        //}

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

        public static async Task<List<Contact>> FindAllAsync(List<int> iIds, bool bIncDeleted = true)
        {
            var lstReturn = new List<Contact>();

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

            lstReturn = lstReturn.OrderBy(r => r.Name_Last).ThenBy(r => r.Name_First).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Contact>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Contact> lstElements)
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

                    using var dB = Settings.Config.DBPooledConnection();

                    //dB.ChangeTracker.AutoDetectChangesEnabled = true;

                    foreach (var efmObject in await dB.tblContacts.AsNoTracking().ToListAsync())
                    {
                        var obj = new Contact(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
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

        public static async Task<List<Contact>> ListAsync(List<Filter<FilterOption>>? lstFilters = null, bool bClearCache = false)
        {
            return (await ListPagedAsync(lstFilters: lstFilters, iPageSize: 1000000, bClearCache: bClearCache)).Elements;
        }

        public static async Task<PagedData<Contact>> ListPagedAsync(List<Filter<FilterOption>>? lstFilters = null, int iPageSize = 25, int iPage = 1, List<SortOption>? lstOrdering = null, bool bClearCache = false)
        {
            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

            var predicate = CompileWhere(lstFilters);

            var lstElements = (await Global_ListAsync()).Values.AsQueryable().AsExpandable().Where(predicate).ToList();

            var objPagedData = new PagedData<Contact>(lstElements.Count, iPageSize, iPage);

            var strOrderBy = SortOption.CompileOrderBy<Contact>(lstOrdering, OrderByDefault, OrderByDefaultDB, false);

            objPagedData.Elements = lstElements.AsQueryable().OrderBy(strOrderBy).Skip(objPagedData.ElementsToSkip).Take(objPagedData.ElementsToTake).ToList();

            return objPagedData;
        }

        private static Expression<Func<Contact, bool>> CompileWhere(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<Contact>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<Contact>(false); }

            if (lstFilters == null) { return predicate; }

            try
            {
                foreach (var vFilter in lstFilters)
                {
                    switch (vFilter.Option)
                    {
                        case FilterOption.Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                if (vFilter.Exclude)
                                {
                                    predicate = predicate.And(r => !lstIds.Contains(r.Id));
                                }
                                else
                                {
                                    predicate = predicate.And(r => lstIds.Contains(r.Id));
                                }
                            }
                            break;

                        case FilterOption.Name_First:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name_First.ToLower().Contains(strSearch));
                            }
                            break;
                        case FilterOption.Name_Last:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name_Last.ToLower().Contains(strSearch));
                            }
                            break;
                        case FilterOption.Name_Full:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name_First.ToLower().Contains(strSearch) || r.Name_Middle.ToLower().Contains(strSearch) || r.Name_Last.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Company:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.CompanyName.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Category:
                            {
                                if (vFilter.Value != null)
                                {
                                    var enumValue = (Enums.Contacts_Contact_Category)vFilter.Value;
                                    if (vFilter.Exclude)
                                    {
                                        predicate = predicate.And(r => !r.Categories.Contains(enumValue));
                                    }
                                    else
                                    {
                                        predicate = predicate.And(r => r.Categories.Contains(enumValue));
                                    }
                                }
                            }
                            break;
                        case FilterOption.Categories:
                            {
                                if (vFilter.Value != null)
                                {
                                    var lst = (List<Enums.Contacts_Contact_Category>)vFilter.Value;
                                    if (vFilter.Exclude)
                                    {
                                        predicate = predicate.And(r => !r.Categories.Intersect(lst).Any());
                                    }
                                    else
                                    {
                                        predicate = predicate.And(r => r.Categories.Intersect(lst).Any());
                                    }
                                }
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
                _ = Error.Exception(typeof(Contact).ToString(), "CompileWhere(List<Filter<FilterOption>>)", ex);
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
                    case nameof(Name_First):
                        strObject = nameof(Name_First);
                        strDB = nameof(tblContact.tblContact_firstName);
                        break;
                    case nameof(Name_Last):
                        strObject = nameof(Name_Last);
                        strDB = nameof(tblContact.tblContact_lastName);
                        break;
                    case nameof(CompanyName):
                        strObject = nameof(CompanyName);
                        strDB = nameof(tblContact.tblContact_companyName);
                        break;

                    case nameof(Created_UTC):
                        strObject = nameof(Created_UTC);
                        strDB = nameof(tblContact.tblContact_createdUTC);
                        break;
                    case nameof(Edited_UTC):
                        strObject = nameof(Edited_UTC);
                        strDB = nameof(tblContact.tblContact_editedUTC);
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
                _ = Error.Exception(typeof(Contact).ToString(), "OrderByConvert(string, bool)", ex,
                    "strFieldName: " + strFieldName.ToString() +
                    ", bDBVersion: " + bDBVersion);
                return "";
            }
        }

        #endregion lists
    }


    public class ZohoContactRequest
    {
        public ZohoContactRequest()
        {
            Account_Name = new AccountName();
        }
        public string id { get; set; }
        public string First_Name { get; set; }
        public string Full_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Method { get; set; }
        public AccountName Account_Name { get; set; }
    }

    public class AccountName
    {
        public AccountName()
        {
            name = "Connect Us Developer";
            id = null;
        }
        public string? name { get; set; }
        public string? id { get; set; }
    }

    public class ContactRequest
    {
        public ContactRequest(List<ZohoContactRequest> args)
        {
            data = args ;
        }
        public List<ZohoContactRequest> data { get; set; }
    }

}

