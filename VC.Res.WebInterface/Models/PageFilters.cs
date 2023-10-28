namespace VC.Res.WebInterface.Models.PageFilters
{
    public class Contacts_List
    {
        public string Name { get; set; } = "";
        public string Company { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telephone { get; set; } = "";
        public int[] Categories { get; set; } = Array.Empty<int>();

        public int CurrentPage { get; set; } = 1;
    }

    public class Premises_List
    {
        public string Name { get; set; } = "";

        public int Group_Id { get; set; } = 0;

        public int Country_Id { get; set; } = 0;

        public int CurrentPage { get; set; } = 1;
    }

    public class Premises_Groups_List
    {
        public string Name { get; set; } = "";

        public int CurrentPage { get; set; } = 1;
    }

    public class Quote_List
    {
        public DateTime Arrival_Date { get; set; } = DateTime.UtcNow;

        public bool Flexible_Arrival { get; set; } = false;

        public bool Include_Unavailable { get; set; } = false;

        public DateTime? Secondary_Date { get; set; }

        public int Num_Nights { get; set; } = 7;

        public int Country_Id { get; set; } = 0;

        public int Region_Id { get; set; } = 0;

        public int Currency_Id { get; set; } = 0;

        public int Num_People { get; set; } = 0;

        public bool Include_Minimum { get; set; } = false;

        public int[] Features { get; set; } = Array.Empty<int>();
    }

    public class Tags_List
    {
        public Core.Enums.Common_Tag_Type Type { get; set; } = Core.Enums.Common_Tag_Type.ContactService;
    }

    public class Users_List
    {
        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public bool Admin { get; set; } = false;

        public int CurrentPage { get; set; } = 1;
    }
}
