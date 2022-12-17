using System;
using System.Windows;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Services
{
    /// <summary>
    /// Сервис навигации
    /// </summary>
    public class AppWindowsService
    {
        private readonly IServiceProvider _ServiceProvider;

        public AppWindowsService(IServiceProvider ServiceProvider)
        {
            _ServiceProvider = ServiceProvider;
        }

        public Window ActiveWindow => App.ActiveWindow;

        public T GetWindow<T>() where T: Window => _ServiceProvider.GetRequiredService<T>();

        public T GetDialogWindow<T>() where T : Window
        {
            var window = _ServiceProvider.GetRequiredService<T>();
            window.Owner = ActiveWindow;
            return window;
        }

        public ViewModel GetViewModel<T>() where T: ViewModel => _ServiceProvider.GetRequiredService<T>();

        public AddJoyBinding AddJoyBinding => _ServiceProvider.GetRequiredService<AddJoyBinding>();

        public EditPatternWindow EditPatternWindow => _ServiceProvider.GetRequiredService<EditPatternWindow>();
    }
}
