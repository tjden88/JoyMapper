using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Helpers
{
    public static class JoyBindingHelper
    {
        public static JoyBindingBase GetBinding()
        {
            var appWindowsService = App.Services.GetRequiredService<AppWindowsService>();

            var wnd = appWindowsService.AddJoyBinding;

            wnd.Owner = appWindowsService.ActiveWindow;

            return wnd.ShowDialog() == true 
                ? wnd.ViewModel.JoyBinding 
                : null;
        }
    }
}
