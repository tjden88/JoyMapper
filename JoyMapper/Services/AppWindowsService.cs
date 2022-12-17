using System;
using System.Windows;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

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

        public AddJoyBinding AddJoyBinding => _ServiceProvider.GetRequiredService<AddJoyBinding>();
    }
}
