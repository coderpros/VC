using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises.Seasons
{
    public class Date
    {
        #region Properties

        public Guid Id_Guid { get; private set; } = Guid.NewGuid();

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Season_Id { get; private set; } = 0;

        /// <summary>
        ///  Date the season starts
        /// </summary>
        public DateTime Start { get; set; } = new DateTime();

        /// <summary>
        /// date the season ends (inclusive)
        /// </summary>
        public DateTime End { get; set; } = new DateTime();

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Date() { }

        internal Date(tblPropertySeasonDate efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Properties

        #region Private Properties-Loaders

        private bool Load(tblPropertySeasonDate efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertySeasonDate_id;

                    Season_Id = efmObject.tblPropertySeason_id;

                    Start = efmObject.tblPropertySeasonDate_from;
                    End = efmObject.tblPropertySeasonDate_to;

                    Created_UTC = efmObject.tblPropertySeasonDate_createdUTC;
                    Created_By = efmObject.tblPropertySeasonDate_createdBy;

                    Edited_UTC = efmObject.tblPropertySeasonDate_editedUTC;
                    Edited_By = efmObject.tblPropertySeasonDate_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertySeasonDate)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private properties-loaders



        #endregion private properties


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_SeasonAsync(int iSeasonId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertySeasonDates.Where(r => r.tblPropertySeason_id == iSeasonId).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Date).ToString(), "DeleteFullBy_SeasonAsync(int, string)", ex,
                    "iSeasonId: " + iSeasonId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions



        #endregion public functions
    }
}
