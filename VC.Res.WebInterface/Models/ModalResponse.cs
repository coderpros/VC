namespace VC.Res.WebInterface.Models
{
    public class ModalResponse
    {
        public bool Cancelled { get; set; } = false;

        public bool Successful { get; set; } = false;

        public ToastNotification.Types Message_Type { get; set; } = ToastNotification.Types.General;

        public string Message_Wording { get; set; } = "";

        public int? Selected_Id { get; set; } = null;

        public List<int> Selected_Ids { get; set; } = new();

        public string Selected_Value { get; set; } = "";

        public List<string> Selected_Values { get; set; } = new();
    }
}
