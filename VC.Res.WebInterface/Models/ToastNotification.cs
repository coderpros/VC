namespace VC.Res.WebInterface.Models
{
    public class ToastNotification
    {
        public enum Types
        {
            General,
            Success,
            Warning,
            Error
        };


        public Types Type { get; set; } = Types.General;

        public string? Title { get; set; } = null;

        public string Message { get; set; } = "";
    }
}
