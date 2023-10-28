using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;

namespace VC.Res.WebInterface.Bases
{
    public class Modal : Component
    {
        [Parameter] public EventCallback<Models.ModalResponse> OnClosed { get; set; }

        public void Close()
        {
            _visible = false;
            StateHasChanged();
        }

        protected async Task CancelClickAsync()
        {
            Close();
            await OnClosed.InvokeAsync(new Models.ModalResponse { Cancelled = true });
        }

        protected async Task OverlayClickAsync(OverlayModalClickEventArgs args)
        {
            await CancelClickAsync();
        }
    }
}
