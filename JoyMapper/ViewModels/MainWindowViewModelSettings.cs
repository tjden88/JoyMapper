using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using JoyMapper.Models;
using JoyMapper.Views;
using WPR;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels;

public partial class MainWindowViewModel
{
    private const string DonateLink = "https://www.tinkoff.ru/rm/dultsev.denis1/G2APs2254";

    private const string HomeUrl = "https://github.com/tjden88/JoyMapper";

    private const string ReportProblemAddress = "https://github.com/tjden88/JoyMapper/discussions/6";

    public string LastUpdateReleaseNotes;
    public string UpdateDownloadUrl;

    #region Props

    #region DoublePressDelay : int - Интервал двойного нажатия

    /// <summary>Интервал двойного нажатия</summary>
    public int DoublePressDelay
    {
        get => _DataManager.AppSettings.DoublePressDelay;
        set
        {
            if(Equals(_DataManager.AppSettings.DoublePressDelay, value)) return;
            _DataManager.AppSettings.DoublePressDelay = value;
            _DataManager.SaveData();
            OnPropertyChanged(nameof(DoublePressDelay));
        }
    }

    #endregion

    #region LongPressDelay : int - Интервал длинного нажатия

    /// <summary>Интервал длинного нажатия</summary>
    public int LongPressDelay
    {
        get => _DataManager.AppSettings.LongPressDelay;
        set
        {
            if (Equals(_DataManager.AppSettings.LongPressDelay, value)) return;
            _DataManager.AppSettings.LongPressDelay = value;
            _DataManager.SaveData();
            OnPropertyChanged(nameof(LongPressDelay));
        }
    }

    #endregion

    #region CurrentColorTheme : ColorTheme - Текущая цветовая схема

    /// <summary>Текущая цветовая схема</summary>
    private ColorTheme _CurrentColorTheme;

    /// <summary>Текущая цветовая схема</summary>
    public ColorTheme CurrentColorTheme
    {
        get => _CurrentColorTheme;
        set => IfSet(ref _CurrentColorTheme, value).Then(v =>
        {
            v.SetTheme();
            _DataManager.AppSettings.CurrentColorCheme = v.Id;
            _DataManager.SaveData();
        });
    }

    #endregion

    #region IsNewVersionAvaliable : bool - Доступна ли новая версия

    /// <summary>Доступна ли новая версия</summary>
    private bool _IsNewVersionAvaliable;

    /// <summary>Доступна ли новая версия</summary>
    public bool IsNewVersionAvaliable
    {
        get => _IsNewVersionAvaliable;
        set => Set(ref _IsNewVersionAvaliable, value);
    }

    #endregion

        

    public IEnumerable<ColorTheme> ColorThemes => new List<ColorTheme>()
    {
        new ()
        {
            Id = 0,
            PrimaryColor = (Color) ColorConverter.ConvertFromString("#3F51B5")!,
            AccentColor = (Color) ColorConverter.ConvertFromString("#FF5722")!,
        }, 
        new ()
        {
            Id = 1,
            PrimaryColor = (Color) ColorConverter.ConvertFromString("#00838f")!,
            AccentColor = (Color) ColorConverter.ConvertFromString("#757575")!,
        },
        new ()
        {
            Id = 2,
            PrimaryColor = (Color) ColorConverter.ConvertFromString("#757575")!,
            AccentColor = (Color) ColorConverter.ConvertFromString("#26c6da")!,
        },
        new ()
        {
            Id = 3,
            PrimaryColor = (Color) ColorConverter.ConvertFromString("#5d4037")!,
            AccentColor = (Color) ColorConverter.ConvertFromString("#81c784")!,
        },

    };

    #endregion


    #region Command GoToHomepageCommand - Перейти на страницу проекта

    /// <summary>Перейти на страницу проекта</summary>
    private Command _GoToHomepageCommand;

    /// <summary>Перейти на страницу проекта</summary>
    public Command GoToHomepageCommand => _GoToHomepageCommand
        ??= new Command(OnGoToHomepageCommandExecuted, CanGoToHomepageCommandExecute, "Перейти на страницу проекта");

    /// <summary>Проверка возможности выполнения - Перейти на страницу проекта</summary>
    private bool CanGoToHomepageCommandExecute() => true;

    /// <summary>Логика выполнения - Перейти на страницу проекта</summary>
    private void OnGoToHomepageCommandExecuted() => OpenWebPage(HomeUrl);

    #endregion

    #region Command ReportProblemCommand - Сообщить о проблеме

    /// <summary>Сообщить о проблеме</summary>
    private Command _ReportProblemCommand;

    /// <summary>Сообщить о проблеме</summary>
    public Command ReportProblemCommand => _ReportProblemCommand
        ??= new Command(OnReportProblemCommandExecuted, CanReportProblemCommandExecute, "Сообщить о проблеме");

    /// <summary>Проверка возможности выполнения - Сообщить о проблеме</summary>
    private bool CanReportProblemCommandExecute() => true;

    /// <summary>Логика выполнения - Сообщить о проблеме</summary>
    private void OnReportProblemCommandExecuted() => OpenWebPage(ReportProblemAddress);

    #endregion

    #region Command MakeDonateCommand - Поддержать разработчика

    /// <summary>Поддержать разработчика</summary>
    private Command _MakeDonateCommand;

    /// <summary>Поддержать разработчика</summary>
    public Command MakeDonateCommand => _MakeDonateCommand
        ??= new Command(OnMakeDonateCommandExecuted, CanMakeDonateCommandExecute, "Поддержать разработчика");

    /// <summary>Проверка возможности выполнения - Поддержать разработчика</summary>
    private bool CanMakeDonateCommandExecute() => true;

    /// <summary>Логика выполнения - Поддержать разработчика</summary>
    private void OnMakeDonateCommandExecuted() => OpenWebPage(DonateLink);

    #endregion

    #region AsyncCommand CheckUpdatesCommand - Проверить обновления при запуске

    /// <summary>Проверить обновления при запуске</summary>
    private AsyncCommand _CheckUpdatesCommand;

    /// <summary>Проверить обновления при запуске</summary>
    public AsyncCommand CheckUpdatesCommand => _CheckUpdatesCommand
        ??= new AsyncCommand(OnCheckUpdatesCommandExecutedAsync, CanCheckUpdatesCommandExecute, "Проверить обновления при запуске");

    /// <summary>Проверка возможности выполнения - Проверить обновления при запуске</summary>
    private bool CanCheckUpdatesCommandExecute() => true;

    /// <summary>Логика выполнения - Проверить обновления при запуске</summary>
    private async Task OnCheckUpdatesCommandExecutedAsync(CancellationToken cancel)
    {
        var updateAvaliable = await _AppUpdateService.CheckUpdate();
        IsNewVersionAvaliable = updateAvaliable;
        if (!updateAvaliable) return;
        LastUpdateReleaseNotes = await _AppUpdateService.GetLastReleaseNotes();
        UpdateDownloadUrl = await _AppUpdateService.GetDownloadLink();

        WPRMessageBox.Bubble(App.ActiveWindow, "Новая версия программы доступна!", "Подробнее", ShowUpdateWindow);

    }

    #endregion

    #region Command ShowUpdateWindowCommand - Показать окно обновления

    /// <summary>Показать окно обновления</summary>
    private Command _ShowUpdateWindowCommand;

    /// <summary>Показать окно обновления</summary>
    public Command ShowUpdateWindowCommand => _ShowUpdateWindowCommand
        ??= new Command(OnShowUpdateWindowCommandExecuted, CanShowUpdateWindowCommandExecute, "Показать окно обновления");

    /// <summary>Проверка возможности выполнения - Показать окно обновления</summary>
    private bool CanShowUpdateWindowCommandExecute() => IsNewVersionAvaliable;

    /// <summary>Логика выполнения - Показать окно обновления</summary>
    private void OnShowUpdateWindowCommandExecuted() => ShowUpdateWindow(true);

    #endregion

    private void OpenWebPage(string Address)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Address,
            UseShellExecute = true
        });
    }

    private async void ShowUpdateWindow(bool Clicked)
    {
        if (!Clicked) return;

        var wnd = _AppWindowsService.GetDialogWindow<UpdateWindow>();
        var vm = wnd.ViewModel;
        vm.DownloadLink = await _AppUpdateService.GetDownloadLink();
        vm.ReleaseNotes = await _AppUpdateService.GetLastReleaseNotes();

        wnd.ShowDialog();

    }


}