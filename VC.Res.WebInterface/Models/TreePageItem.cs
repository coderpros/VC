namespace VC.Res.WebInterface.Models
{
    public class TreePageItem
    {
        public int Id { get; set; } = 0;
        public int? ParentId { get; set; } = null;
        public bool HasChildren { get; set; } = false;
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string Url { get; set; } = "";
        public int Order { get; set; } = 0;
        public bool Enabled { get; set; } = false;
        public bool Visible { get; set; } = false;
        public string LastEdited { get; set; } = "";
        public bool CanAdd { get; set; } = false;
        public bool Expanded { get; set; } = false;
    }
}
