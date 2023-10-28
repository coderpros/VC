using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC.Res.Core.Models
{
    public class ZohoResponseModel
    {

    }
    public class CreatedBy
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Datum
    {
        public string code { get; set; }
        public Details details { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }

    public class Details
    {
        public DateTime Modified_Time { get; set; }
        public ModifiedBy Modified_By { get; set; }
        public DateTime Created_Time { get; set; }
        public string id { get; set; }
        public CreatedBy Created_By { get; set; }
    }

    public class ModifiedBy
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class ContactResponse
    {
        public List<Datum> data { get; set; }
    }
}
