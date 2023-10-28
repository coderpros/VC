namespace VC.Res.WebInterface.Services
{
    public class NiceUIService : IDisposable
    {
        public event Func<Models.ToastNotification, Task> OnToastShow;
        public event Func<Models.ToastNotification, Task> OnToastShowAfterReload;

        public event Func<Task> OnSpinnerShow;
        public event Func<Task> OnSpinnerHide;

        public async Task ToastShowAsync(Models.ToastNotification msg)
        {
            if (OnToastShow != null)
            {
                await OnToastShow.Invoke(msg);
            }
        }

        public async Task ToastShowAfterReloadAsync(Models.ToastNotification msg)
        {
            if (OnToastShowAfterReload != null)
            {
                await OnToastShowAfterReload.Invoke(msg);
                //await Task.Delay(20); // small delay to let things catch up/take effect
            }
        }

        public async Task SpinnerShowAsync()
        {
            if (OnSpinnerShow != null)
            {
                await OnSpinnerShow.Invoke();
                await Task.Delay(5); // small delay to let things catch up/take effect
            }
        }

        public async Task SpinnerHideAsync()
        {
            if (OnSpinnerHide != null)
            {
                await OnSpinnerHide.Invoke();
            }
        }

        public void Dispose()
        {

        }
    }
}
