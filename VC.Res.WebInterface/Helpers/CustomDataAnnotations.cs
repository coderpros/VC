using System.ComponentModel.DataAnnotations;

namespace VC.Res.WebInterface.Helpers.CustomDataAnnotations
{
    public class PasswordLength : RegularExpressionAttribute
    {
        public PasswordLength()
            : base("^.{10,30}$")
        {
        }
    }

    public class PasswordComplexity : RegularExpressionAttribute
    {
        public PasswordComplexity()
            : base("^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z]).*$")
        {
        }
    }
}
