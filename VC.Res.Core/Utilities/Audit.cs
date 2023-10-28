using Newtonsoft.Json;
using VC.Res.Core.Database;

namespace VC.Res.Core.Utilities
{
    public class Audit
    {
        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public Enums.Utilities_Audit_Action Action { get; private set; } = Enums.Utilities_Audit_Action.Unknown;

        public int ForeignId { get; private set; } = 0;

        public Type? DataType { get; private set; } = null;

        public object? Data_New { get; private set; } = null;

        public object? Data_Old { get; private set; } = null;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Created_Local { get; private set; } = DateTime.Now;

        public string Created_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        private Audit() { }

        //private Audit(tblSysAudit efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private bool Load(tblSysAudit efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblSysAudit_id;
                    Action = (Enums.Utilities_Audit_Action)efmObject.tblSysAudit_action;
                    ForeignId = efmObject.tblSysAudit_foreignId;

                    DataType = Type.GetType(efmObject.tblSysAudit_type);

                    var contractResolver = new CustomContractResolver();
                    var settings = new JsonSerializerSettings { ContractResolver = contractResolver };

                    if (!string.IsNullOrWhiteSpace(efmObject.tblSysAudit_newData))
                    {
                        Data_New = JsonConvert.DeserializeObject(efmObject.tblSysAudit_newData, DataType, settings);
                    }

                    if (!string.IsNullOrWhiteSpace(efmObject.tblSysAudit_oldData))
                    {
                        Data_Old = JsonConvert.DeserializeObject(efmObject.tblSysAudit_oldData, DataType, settings);
                    }

                    Created_UTC = efmObject.tblSysAudit_createdUTC;
                    Created_Local = efmObject.tblSysAudit_createdLocal;
                    Created_By = efmObject.tblSysAudit_createdBy;

                    Loaded = true;
                    bReturn = true;

                    contractResolver = null;
                    settings = null;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblSysAudit)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        #endregion private functions


        #region Public Functions

        public static async Task<int?> CreateAsync(Enums.Utilities_Audit_Action enumAction, int iForeignId, object? objNewData = null, object? objOldData = null, string strCreatedBy = "")
        {
            if (objNewData == null && objOldData == null) { return null; }

            int? iReturn = null;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var contractResolver = new CustomContractResolver();
                var settings = new JsonSerializerSettings { ContractResolver = contractResolver };

                var efmObject = new tblSysAudit
                {
                    tblSysAudit_action = (int)enumAction,
                    tblSysAudit_foreignId = iForeignId,
                    tblSysAudit_createdUTC = DateTime.UtcNow,
                    tblSysAudit_createdLocal = DateTime.Now,
                    tblSysAudit_createdBy = strCreatedBy
                };

                if (objNewData != null)
                {
                    efmObject.tblSysAudit_type = objNewData.GetType().ToString();
                    efmObject.tblSysAudit_newData = JsonConvert.SerializeObject(objNewData, settings);
                }

                if (objOldData != null)
                {
                    efmObject.tblSysAudit_type = objOldData.GetType().ToString();
                    efmObject.tblSysAudit_oldData = JsonConvert.SerializeObject(objOldData, settings);
                }

                _ = dB.tblSysAudits.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    iReturn = efmObject.tblSysAudit_id;
                }

                efmObject = null;

                settings = null;
                contractResolver = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Audit).ToString(), "CreateAsync(Enums.Utilities_Audit_Action, int, object, object, string)", ex,
                                            "enumAction: " + enumAction.ToString() +
                                            ", iForeignId: " + iForeignId.ToString() +
                                            ", strCreatedBy: " + strCreatedBy.ToString());
                return iReturn;
            }

            return iReturn;
        }

        #endregion public functions


        #region Finders



        #endregion finders


        #region Lists



        #endregion lists
    }
}
