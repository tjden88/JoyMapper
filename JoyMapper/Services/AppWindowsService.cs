using System;
using System.Windows;
using JoyMapper.Models.JoyBindings.Base;
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
        private static Window ActiveWindow => App.ActiveWindow;


        public AppWindowsService(IServiceProvider ServiceProvider)
        {
            _ServiceProvider = ServiceProvider;
        }


        /// <summary> Получить экземпляр окна из коллекции сервисов </summary>
        public T GetWindow<T>() where T: Window => _ServiceProvider.GetRequiredService<T>();


        /// <summary> Получить экземпляр окна для диалога с настроенным родителем из коллекции сервисов </summary>
        public T GetDialogWindow<T>() where T : Window
        {
            var window = _ServiceProvider.GetRequiredService<T>();
            window.Owner = ActiveWindow;
            return window;
        }


        /// <summary> Получить экземпляр вьюмодели представления из коллекции сервисов </summary>
        public T GetViewModel<T>() where T: ViewModel => _ServiceProvider.GetRequiredService<T>();


        /// <summary> Показать окно выбора и получить привязку кнопки </summary>
        public JoyBindingBase GetJoyBinding()
        {
            var wnd = GetDialogWindow<AddJoyBinding>();
            return wnd.ShowDialog() == true
                ? wnd.ViewModel.JoyBinding
                : null;
        }
    }
}
