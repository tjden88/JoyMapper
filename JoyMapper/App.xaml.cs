using System;
using System.Linq;
using System.Windows;
using JoyMapper.Services;
using JoyMapper.Services.Interfaces;
using JoyMapper.ViewModels.Windows;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary> Версия приложения </summary>
        internal const string AppVersion = "1.3";

        /// <summary> Менеджер данных профилей текущей сессии </summary>
        internal static DataManager DataManager { get; } = new();

        /// <summary> Активное окно </summary>
        internal static Window ActiveWindow => Current.Windows.Cast<Window>().First(w => w.IsActive);

        /// <summary> Сервис проверки обновлений </summary>
        internal static UpdateChecker UpdateChecker { get; } = new();


        private static IServiceProvider _Services;

        public static IServiceProvider Services => _Services ??= ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton<AppWindowsService>()
                .AddTransient<AddJoyBinding>()
                .AddTransient<EditPatternWindow>()
            .AddTransient<AddJoyBindingViewModel>()
            .AddTransient<EditPatternViewModel>()
                .AddTransient<IJoystickStateManager, JoystickStateManager>()
            .AddTransient<IJoyBindingsWatcher, JoyBindingsWatcher>()

            ;

            return serviceCollection.BuildServiceProvider();
        }
    }

}
