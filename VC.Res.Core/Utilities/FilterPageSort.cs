using System.Text;
using Newtonsoft.Json;

namespace VC.Res.Core.Utilities
{
    public class DateFilter
    {
        [JsonProperty]
        public DateTime? From { get; private set; } = null;

        [JsonProperty]
        public DateTime? To { get; private set; } = null;

        [JsonProperty]
        public DateTime? Equal { get; private set; } = null;

        [JsonProperty]
        public bool? EqualNull { get; private set; } = null;

        private DateFilter() { }

        public DateFilter(DateTime? dtFrom = null, DateTime? dtTo = null, DateTime? dtEqual = null, bool? bEqualNull = null)
        {
            From = dtFrom;
            To = dtTo;
            Equal = dtEqual;

            //if (dtTo.HasValue)
            //{
            //    if (dtTo.Value.Hour == 0 && dtTo.Value.Minute == 0)
            //    {
            //        To = dtTo.Value.AddDays(1).AddSeconds(-1);
            //    }
            //}

            EqualNull = bEqualNull;
        }
    }


    public class FilterList<T>
    {
        [JsonProperty]
        public List<Filter<T>> Filters { get; private set; }

        public FilterList()
        {
            Filters = new List<Filter<T>>();
        }

        public void Add(T enumFilterOption, object? objValue, bool bExclude = false)
        {
            Filters.Add(new Filter<T>(enumFilterOption, objValue, bExclude));
        }
    }

    public class Filter<T>
    {
        [JsonProperty]
        public T Option { get; private set; }

        [JsonProperty]
        public object? Value { get; private set; } = null;

        [JsonProperty]
        public bool Exclude { get; private set; } = false;

        private Filter() { }

        public Filter(T enumFilterOption, object? objValue, bool bExclude = false)
        {
            Option = enumFilterOption;
            Value = objValue;
            Exclude = bExclude;
        }

        public Filter(object enumFilterOption, object? objValue, bool bExclude = false)
        {
            Option = (T)enumFilterOption;
            Value = objValue;
            Exclude = bExclude;
        }

        public string Value_String()
        {
            var strReturn = Value?.ToString();

            if (string.IsNullOrWhiteSpace(strReturn)) { return ""; }

            return strReturn;
        }

        public bool Value_Bool()
        {
            var bReturn = false;

            try
            {
                if (Value != null)
                {
                    _ = bool.TryParse(Value.ToString(), out bReturn);
                }
            }
            catch (Exception)
            {
                return bReturn;
            }

            return bReturn;
        }

        public int? Value_IntNullable()
        {
            int? iReturn = null;

            try
            {
                if (Value != null)
                {
                    if (int.TryParse(Value.ToString(), out var iTemp))
                    {
                        iReturn = iTemp;
                    }
                }
            }
            catch (Exception)
            {
                return iReturn;
            }

            return iReturn;
        }

        public int Value_Int()
        {
            var iReturn = 0;

            try
            {
                if (Value != null)
                {
                    _ = int.TryParse(Value.ToString(), out iReturn);
                }
            }
            catch (Exception)
            {
                return iReturn;
            }

            return iReturn;
        }

        public DateFilter Value_DateFilter()
        {
            if (Value != null)
            {
                if (Value is DateFilter filter)
                {
                    return filter;
                }

                if (Value is Newtonsoft.Json.Linq.JObject json)
                {
                    return JsonConvert.DeserializeObject<DateFilter>(json.ToString(), new JsonSerializerSettings
                    {
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                    });
                }
            }

            return new DateFilter();
        }

        public List<int> Value_ListInt()
        {
            if (Value != null)
            {
                if (Value is List<int> filter)
                {
                    return filter;
                }

                if (Value is Newtonsoft.Json.Linq.JObject json)
                {
                    return JsonConvert.DeserializeObject<List<int>>(json.ToString());
                }
            }

            return new List<int>();
        }
    }

    public class SortOption
    {
        public string Field { get; private set; } = "";

        public Enums.Utilities_SortOption_Order Sort { get; private set; } = Enums.Utilities_SortOption_Order.Asc;

        public SortOption(string strField, Enums.Utilities_SortOption_Order enumOrder = Enums.Utilities_SortOption_Order.Asc)
        {
            Field = strField;
            Sort = enumOrder;

        }

        internal static string CompileOrderBy<T>(List<SortOption>? lstSortOptions, string strDefault, string strDefaultDB, bool bDB, string strConvertFieldMethod = "OrderByConvert")
        {
            // if object type is null we have no choice but to return nothing
            try
            {
                var objType = typeof(T);

                if (objType == null) { return ""; }

                var strReturn = new StringBuilder();

                if (lstSortOptions != null)
                {
                    // get the method to call to convert field name
                    var miOrderByConvert = objType.GetMethod(strConvertFieldMethod, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                    if (miOrderByConvert != null)
                    {
                        foreach (var vSort in lstSortOptions)
                        {
                            var strField = "";

                            strField = miOrderByConvert.Invoke(null, new object[] { vSort.Field, bDB })?.ToString();

                            // check a field name was returned
                            if (string.IsNullOrWhiteSpace(strField)) { continue; }

                            if (vSort.Sort == Enums.Utilities_SortOption_Order.Asc)
                            {
                                strField += " ASC";
                            }
                            else
                            {
                                strField += " DESC";
                            }

                            if (strReturn.Length > 0) { _ = strReturn.Append(", "); }

                            _ = strReturn.Append(strField);
                        }
                    }
                }

                // set default sort if none added
                if (strReturn.Length < 1)
                {
                    if (bDB)
                    {
                        _ = strReturn.Append(strDefaultDB);
                    }
                    else
                    {
                        _ = strReturn.Append(strDefault);
                    }
                }

                return strReturn.ToString();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(SortOption).ToString(), "CompileOrderBy(List<SortOption>, string, string, bool, string)", ex);
                if (bDB)
                {
                    return strDefaultDB;
                }
                else
                {
                    return strDefault;
                }
            }
        }
    }

    public class PagedData<T>
    {
        #region Private Properties

        // Require values for calculations
        //private int _i_TotalNoItems = 0;

        //private int _i_PageSize = 25;

        //private int _i_CurrentPage = 1;

        // calculated values
        //private int _i_TotalPages = 0;

        private int _i_CurrentPageIndex = 0;

        //private int _i_Elements_Skip = 0;
        // number of items to take, also doubles as the number of items to show which is different to the page size as that was
        // the desired number to show (there may be less than page size)
        //private int _i_Elements_Take = 0;

        //private int _i_Elements_Start = 0;
        //private int _i_Elements_End = 0;

        #endregion private properties

        #region Public Properties

        /// <summary>
        /// The overall total number of elements found from query
        /// </summary>
        [JsonProperty]
        public int TotalItemsFound { get; private set; } = 0;

        /// <summary>
        /// The currently selected page size (may be larger than number of elements returned)
        /// </summary>
        [JsonProperty]
        public int PageSize { get; private set; } = 25;

        /// <summary>
        /// The current page of elements being returned
        /// </summary>
        [JsonProperty]
        public int CurrentPage { get; private set; } = 1;

        /// <summary>
        /// Actual list of elements to show
        /// </summary>
        public List<T> Elements { get; set; } = new List<T>();

        public List<T> ExportElements { get; set; } = new List<T>();

        /// <summary>
        /// How many pages of elements there are
        /// </summary>
        [JsonProperty]
        public int TotalPages { get; private set; } = 0;

        /// <summary>
        /// How many elements to skip before selection of elements is to begin
        /// </summary>
        // starting point from where to take items from (e.g if 0, start at index 0 or skip 0 elements)
        [JsonProperty]
        public int ElementsToSkip { get; private set; } = 0;

        /// <summary>
        /// The number of elements to take from overall count (should equal a count of 'Elements')
        /// </summary>
        /// // number of items to take, also doubles as the number of items to show which is different to the page size as that was
        // the desired number to show (there may be less than page size)
        [JsonProperty]
        public int ElementsToTake { get; private set; } = 0;

        /// <summary>
        /// 1 based index of the starting element returned
        /// </summary>
        [JsonProperty]
        public int ElementsRangeStart { get; private set; } = 0;

        /// <summary>
        /// 1 based index of the last element returned
        /// </summary>
        [JsonProperty]
        public int ElementsRangeEnd { get; private set; } = 0;

        #endregion private properties

        #region Constructors

        private PagedData() { }

        public PagedData(int iTotalNoItemsFound, int iPageSize = 25, int iCurrentPage = 1)
        {
            TotalItemsFound = iTotalNoItemsFound;
            PageSize = iPageSize;
            CurrentPage = iCurrentPage;

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            _i_CurrentPageIndex = CurrentPage - 1;

            CalculateValue();
        }

        #endregion constructors

        #region Private Functions

        private void CalculateValue()
        {
            TotalPages = 0;
            //var iNoOfPagesReminder = 0;

            TotalPages = Math.DivRem(TotalItemsFound, PageSize, out var iNoOfPagesReminder);

            if (iNoOfPagesReminder > 0)
            {
                TotalPages++;
            }

            if (_i_CurrentPageIndex > TotalPages)
            {
                // trying to load a page that doesn't exist, go to 0
                _i_CurrentPageIndex = 0;
                CurrentPage = 1;
            }

            // get wanted element range
            ElementsToTake = PageSize;
            ElementsToSkip = _i_CurrentPageIndex * PageSize;

            if ((ElementsToSkip + (PageSize - 1)) >= TotalItemsFound)
            {
                ElementsToTake = TotalItemsFound - ElementsToSkip;
            }

            ElementsRangeStart = ElementsToSkip + 1;
            ElementsRangeEnd = ElementsToSkip + ElementsToTake;
        }

        #endregion private functions


        #region Public Functions

        public List<Models.PageNumber> PagingData(string strClassPrev = "previous", string strClassPage = "pageNumber", string strClassPageCurrent = "pageNumber", string strClassNext = "next")
        {
            var lstReturn = new List<Models.PageNumber>();

            if (CurrentPage != 1)
            {
                lstReturn.Add(new Models.PageNumber
                {
                    Text = "< Previous",
                    Title = "Go to previous page",
                    Page = (CurrentPage - 1),
                    Class = strClassPrev
                });
            }

            for (var i = 1; i <= TotalPages; i++)
            {
                if (i == CurrentPage)
                {
                    lstReturn.Add(new Models.PageNumber
                    {
                        Text = "<strong>" + i.ToString() + "</strong>",
                        Title = "Go to page " + i.ToString(),
                        Page = i,
                        Class = strClassPageCurrent
                    });
                }
                else
                {
                    lstReturn.Add(new Models.PageNumber
                    {
                        Text = i.ToString(),
                        Title = "Go to page " + i.ToString(),
                        Page = i,
                        Class = strClassPage
                    });
                }
            }

            if (CurrentPage != TotalPages)
            {
                lstReturn.Add(new Models.PageNumber
                {
                    Text = "Next >",
                    Title = "Go to next page",
                    Page = (CurrentPage + 1),
                    Class = strClassNext
                });
            }

            return lstReturn;
        }

        #endregion public functions
    }
}
