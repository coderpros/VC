using Microsoft.AspNetCore.Components;

namespace VC.Res.WebInterface.Bases
{
    public class Component : ComponentBase
    {
        [Inject] public Services.NiceUIService? NiceUIService { get; set; }

        [Parameter] public EventCallback OnChanged { get; set; }

        [Parameter] public int RefreshCount { get; set; } = 0;

        protected bool _processing = false;

        protected bool _visible = false;

        protected int _refreshCount = 0;

        protected async Task<bool> StartProcessingAsync(bool bShowSpinner = true)
        {
            if (_processing) { return false; }

            _processing = true;

            if (NiceUIService != null && bShowSpinner)
            {
                await NiceUIService.SpinnerShowAsync();
            }

            return true;
        }

        protected async Task EndProcessingAsync(bool bShowSpinner = true)
        {
            if (!_processing) { return; }

            if (NiceUIService != null && bShowSpinner)
            {
                await NiceUIService.SpinnerHideAsync();
            }

            _processing = false;
        }
    }
}
