using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC.Res.Core.Settings
{
    public class Variables
    {
        public const bool Errors_Throw = false;

        public const string TimeZone_Default = "GMT Standard Time";


        public const string Encryption_DefaultKey = "bs99Omega";

        public const string Encryption_DefaultPassword = "dh74Alpha";

        public const string Encryption_DefaultSalt = "vc22Theta";


        public const int RecycleBin_EmptyAfterDaysShort = 30;

        public const int RecycleBin_EmptyAfterDaysMedium = 60;

        public const int RecycleBin_EmptyAfterDaysLong = 90;


        public const int CommandTimeout_Short = 300; // 5 minutes

        public const int CommandTimeout_Medium = 600; // 10 minutes

        public const int CommandTimeout_Long = 900; // 15 minutes


        public const int SessionExpire_LongTermDays = 7;

        public const int SessionExpire_ShortTermMinutes = 35;
    }
}
