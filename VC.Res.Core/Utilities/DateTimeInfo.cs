using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC.Res.Core.Utilities
{
    public class DateTimeInfo
    {
        #region Private Properties



        #endregion private properties


        #region Public Properties

        public DateTime UTC { get; private set; }

        public DateTime Local { get; private set; }

        public TimeZoneInfo LocalTimeZone { get; private set; }

        #endregion public properties


        #region Constructors

        /// <summary>
        /// New instance using system default as local timezone with DT set as now
        /// </summary>
        public DateTimeInfo()
        {
            LocalTimeZone = TimeZoneInfo.FindSystemTimeZoneById(Settings.Variables.TimeZone_Default);

            UTC = DateTime.UtcNow;

            Local = TimeZoneInfo.ConvertTimeToUtc(UTC, LocalTimeZone);
        }

        /// <summary>
        /// New instance using a specific timezone with DT set to now
        /// </summary>
        /// <param name="strTimeZone"></param>
        public DateTimeInfo(string strTimeZone)
        {
            LocalTimeZone = TimeZoneInfo.FindSystemTimeZoneById(strTimeZone);

            UTC = DateTime.UtcNow;

            Local = TimeZoneInfo.ConvertTimeFromUtc(UTC, LocalTimeZone);
        }

        public DateTimeInfo(DateTime dtValue, string strTimeZone, bool bUTCValue = false)
        {
            LocalTimeZone = TimeZoneInfo.FindSystemTimeZoneById(strTimeZone);

            if (bUTCValue)
            {
                UTC = DateTime.SpecifyKind(dtValue, DateTimeKind.Utc);

                Local = TimeZoneInfo.ConvertTimeFromUtc(UTC, LocalTimeZone);
            }
            else
            {
                Local = DateTime.SpecifyKind(dtValue, DateTimeKind.Unspecified);

                UTC = TimeZoneInfo.ConvertTime(Local, LocalTimeZone, TimeZoneInfo.Utc);
            }
        }

        #endregion


        #region Private Functions



        #endregion private functions


        #region Public Functions

        public static DateTime GetLocal(DateTime dtUTCValue)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dtUTCValue, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(Settings.Variables.TimeZone_Default));
        }

        public static DateTime GetLocal(DateTime dtUTCValue, string strTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(dtUTCValue, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(strTimeZone));
        }
        public static DateTime GetLocalNow(string strTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById(strTimeZone));
        }

        public static DateTime GetUTC(DateTime dtLocalValue)
        {
            return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dtLocalValue, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById(Settings.Variables.TimeZone_Default), TimeZoneInfo.Utc);
        }

        public static DateTime GetUTC(DateTime dtLocalValue, string strTimeZone)
        {
            return TimeZoneInfo.ConvertTime(DateTime.SpecifyKind(dtLocalValue, DateTimeKind.Unspecified), TimeZoneInfo.FindSystemTimeZoneById(strTimeZone), TimeZoneInfo.Utc);
        }

        public static int Calculate_Quarter_No(DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month < 4)
            {
                return 1;
            }
            else if (dt.Month >= 4 && dt.Month < 7)
            {
                return 2;
            }
            else if (dt.Month >= 7 && dt.Month < 10)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public static (DateTime, DateTime) Calculate_DateRange_Quarter(DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month < 4)
            {
                return (new DateTime(dt.Year, 1, 1), new DateTime(dt.Year, 4, 1).AddDays(-1));
            }
            else if (dt.Month >= 4 && dt.Month < 7)
            {
                return (new DateTime(dt.Year, 4, 1), new DateTime(dt.Year, 7, 1).AddDays(-1));
            }
            else if (dt.Month >= 7 && dt.Month < 10)
            {
                return (new DateTime(dt.Year, 7, 1), new DateTime(dt.Year, 10, 1).AddDays(-1));
            }
            else
            {
                return (new DateTime(dt.Year, 10, 1), new DateTime(dt.Year + 1, 1, 1).AddDays(-1));
            }
        }

        public static (DateTime, DateTime) Calculate_DateRange_PreviousQuarter(DateTime dt)
        {
            // work out the current quarters range
            var dtRange_Current = Calculate_DateRange_Quarter(dt);

            // previous quarter will be the 3 months previous to current
            return (dtRange_Current.Item1.AddMonths(-3), dtRange_Current.Item1.AddDays(-1));
        }

        public static (DateTime, DateTime) Calculate_DateRange_Year(DateTime dt)
        {
            return (new DateTime(dt.Year, 1, 1), new DateTime(dt.Year + 1, 1, 1).AddDays(-1));
        }

        public static (DateTime, DateTime) Calculate_DateRange_PreviousYear(DateTime dt)
        {
            return (new DateTime(dt.Year - 1, 1, 1), new DateTime(dt.Year, 1, 1).AddDays(-1));
        }

        #endregion public functions
    }
}
