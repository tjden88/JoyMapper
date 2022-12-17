using System;
using System.Linq;
using System.Windows;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;
using JoyMapper.ViewModels;
using JoyMapper.ViewModels.UserControls;
using JoyMapper.ViewModels.Windows;
using JoyMapper.Views;
using JoyMapper.Views.UserControls;
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
        internal const string AppVersion = "1.3.3";

        /// <summary> Менеджер данных профилей текущей сессии </summary>
        [Obsolete]
        internal static DataManager DataManager => Services.GetRequiredService<DataManager>();

        /// <summary> Активное окно </summary>
        internal static Window ActiveWindow => Current.Windows.Cast<Window>().First(w => w.IsActive);


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
                .AddSingleton<UpdateChecker>()
                .AddSingleton<DataManager>()
                .AddSingleton<ProfilesManager>()
                .AddSingleton<JoyPatternManager>()
                .AddSingleton<AppWindowsService>()
                .AddSingleton<DataSerializer>()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()
                .AddTransient<AddJoyBinding>()
                .AddTransient<PatternActionView>()
                .AddTransient<EditPattern>()
            .AddTransient<AddJoyBindingViewModel>()
            .AddTransient<EditPatternViewModel>()
            .AddTransient<PatternActionViewModel>()
            .AddTransient<EditProfile>()
            .AddTransient<EditProfileWindowViewModel>()
                .AddTransient<IJoystickStateManager, JoystickStateManager>()
            .AddTransient<IJoyBindingsWatcher, JoyBindingsWatcher>()

            ;

            return serviceCollection.BuildServiceProvider();
        }
    }

}
