using System;
using System.Linq;
using System.Windows;
using JoyMapper.Services;
using JoyMapper.Services.Interfaces;
using JoyMapper.ViewModels;
using JoyMapper.ViewModels.UserControls;
using JoyMapper.ViewModels.Windows;
using JoyMapper.Views;
using JoyMapper.Views.UserControls;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using EditPatternWindow = JoyMapper.Views.Windows.EditPatternWindow;

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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var wnd = Services.GetRequiredService<MainWindow>();
            wnd.Show();
        }


        private static IServiceProvider _Services;

        public static IServiceProvider Services => _Services ??= ConfigureServices();

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton<DataManager>()
                .AddSingleton<AppWindowsService>()
                .AddSingleton<JoyPatternManager>()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()
                .AddTransient<AddJoyBinding>()
                .AddTransient<PatternActionView>()
                .AddTransient<EditPatternWindow>()
            .AddTransient<AddJoyBindingViewModel>()
            .AddTransient<EditPatternViewModel>()
            .AddTransient<PatternActionViewModel>()
                .AddTransient<IJoystickStateManager, JoystickStateManager>()
            .AddTransient<IJoyBindingsWatcher, JoyBindingsWatcher>()

            ;

            return serviceCollection.BuildServiceProvider();
        }
    }

}
