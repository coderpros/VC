using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC.Res.Core.Bases
{
    public abstract class Basic
    {
        #region Properties

        public bool Loaded { get; protected set; } = false;

        public int Id { get; protected set; } = 0;

        #endregion properties


        #region Private Functions

        #region Private Functions-Loaders

        protected abstract Task<bool> LoadAsync(int iId);

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions



        #endregion public functions


        #region Finders



        #endregion finders


        #region Lists



        #endregion lists
    }
}
