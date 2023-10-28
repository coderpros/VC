namespace VC.Res.WebInterface.Models
{
    public class HeaderLink
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public bool Url_Local { get; set; } = true;
        public List<HeaderLink> SubLinks { get; set; } = new List<HeaderLink>();
    }
}
