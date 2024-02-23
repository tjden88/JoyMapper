using System;
using System.Linq;
using System.Windows;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;
using JoyMapper.Services.Listeners;
using JoyMapper.ViewModels;
using JoyMapper.ViewModels.UserControls;
using JoyMapper.ViewModels.Windows;
using JoyMapper.Views;
using JoyMapper.Views.UserControls;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using SharedServices;

namespace JoyMapper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{

    /// <summary> Активное окно </summary>
    internal static Window ActiveWindow => Current.Windows.Cast<Window>().FirstOrDefault(w => w.IsActive);


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
            .AddSingleton<AppUpdateService>()
            .AddSingleton<DataManager>()
            .AddSingleton<ProfilesManager>()
            .AddSingleton<JoyPatternManager>()
            .AddSingleton<ModificatorManager>()
            .AddSingleton<AppWindowsService>()
            .AddSingleton<DataSerializer>()
            .AddSingleton<MainWindow>()
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<AddJoyBinding>()
            .AddTransient<PatternActionView>()
            .AddTransient<EditPattern>()
            .AddTransient<EditModificator>()
            .AddTransient<EditModificatorViewModel>()
            .AddTransient<AddJoyBindingViewModel>()
            .AddTransient<EditPatternViewModel>()
            .AddTransient<PatternActionViewModel>()
            .AddTransient<AudioPlayerViewModel>()
            .AddTransient<EditProfile>()
            .AddTransient<EditProfileWindowViewModel>()
            .AddTransient<UpdateWindow>()
            .AddTransient<UpdateWindow.UpdateWindowViewModel>()
            .AddTransient<JoyBindingView>()
            .AddTransient<AudioPlayerControls>()
            .AddTransient<AudioPlayerControlsViewModel>()
            .AddTransient<JoyBindingViewModel>()
            .AddTransient<KeyCommandsWatcher.KeyCommandsWatcherViewModel>()
            .AddTransient<KeyCommandsWatcher>()
            .AddSingleton<AudioPlayerService>()
            .AddSingleton<IJoystickStateManager, JoystickStateManager>()
            .AddTransient<IJoyBindingListener, JoyBindingListener>()
            .AddTransient<IJoyPatternListener, JoyPatternListener>()
            .AddTransient<IProfileListener, ProfileListener>()
            .AddSingleton<KeyboardSender>()
            ;

        return serviceCollection.BuildServiceProvider();
    }
}