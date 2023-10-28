using System.ComponentModel.DataAnnotations;

namespace VC.Res.WebInterface.Models
{
    public class ZohoContactRequest
    {
        public ZohoContactRequest()
        {
            Account_Name = new AccountName();
        }
        public string Id { get; set; }
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
        public string? name { get; set; }
        public string? id { get; set; }
    }
}
