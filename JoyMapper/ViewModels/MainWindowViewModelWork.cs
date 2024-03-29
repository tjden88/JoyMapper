﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using JoyMapper.Models;
using JoyMapper.Views.Windows;
using WPR.MVVM.Commands.Base;

namespace JoyMapper.ViewModels;

public partial class MainWindowViewModel
{

    #region LogMessages : ObservableCollection<LogMessage> - Лог запущенного профиля

    /// <summary>Лог запущенного профиля</summary>
    private ObservableCollection<LogMessage> _LogMessages = new();

    /// <summary>Лог запущенного профиля</summary>
    public ObservableCollection<LogMessage> LogMessages
    {
        get => _LogMessages;
        set => Set(ref _LogMessages, value);
    }

    #endregion


    #region IsProfileStarted : bool - Запущен ли профиль

    /// <summary>Запущен ли профиль</summary>
    public bool IsProfileStarted => ActiveProfile != null;

    #endregion


    #region ActiveProfile : Profile - Запущенный профиль

    /// <summary>Запущенный профиль</summary>
    private Profile _ActiveProfile;

    /// <summary>Запущенный профиль</summary>
    public Profile ActiveProfile
    {
        get => _ActiveProfile;
        set => IfSet(ref _ActiveProfile, value).CallPropertyChanged(nameof(IsProfileStarted));
    }

    #endregion


    #region Command StartProfileCommand - Запустить профиль

    /// <summary>Запустить профиль</summary>
    private Command _StartProfileCommand;

    /// <summary>Запустить профиль</summary>
    public Command StartProfileCommand => _StartProfileCommand
        ??= new Command(OnStartProfileCommandExecuted, CanStartProfileCommandExecute, "Запустить профиль");

    /// <summary>Проверка возможности выполнения - Запустить профиль</summary>
    private bool CanStartProfileCommandExecute(object p) => p is Profile pr && pr.PatternsIds.Any();

    /// <summary>Логика выполнения - Запустить профиль</summary>
    private void OnStartProfileCommandExecuted(object p)
    {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, () => LogMessages.Clear());
        var profile = (Profile)p;
        ActiveProfile = profile; 
        _ProfileListener.StartListenProfile(profile);
    }

    #endregion


    #region Command StopProfileCommand - Остановить профиль

    /// <summary>Остановить профиль</summary>
    private Command _StopProfileCommand;

    /// <summary>Остановить профиль</summary>
    public Command StopProfileCommand => _StopProfileCommand
        ??= new Command(OnStopProfileCommandExecuted, CanStopProfileCommandExecute, "Остановить профиль");

    /// <summary>Проверка возможности выполнения - Остановить профиль</summary>
    private bool CanStopProfileCommandExecute() => IsProfileStarted;

    /// <summary>Логика выполнения - Остановить профиль</summary>
    private void OnStopProfileCommandExecuted()
    {
        ActiveProfile = null;
        _ProfileListener.StopListenProfile();
    }

    #endregion


    #region Command ShowKeyboardLogCommand - Показать лог клавиатуры

    /// <summary>Показать лог клавиатуры</summary>
    private Command _ShowKeyboardLogCommand;

    /// <summary>Показать лог клавиатуры</summary>
    public Command ShowKeyboardLogCommand => _ShowKeyboardLogCommand
        ??= new Command(OnShowKeyboardLogCommandExecuted, CanShowKeyboardLogCommandExecute, "Показать лог клавиатуры");

    /// <summary>Проверка возможности выполнения - Показать лог клавиатуры</summary>
    private bool CanShowKeyboardLogCommandExecute() => true;

    /// <summary>Логика выполнения - Показать лог клавиатуры</summary>
    private void OnShowKeyboardLogCommandExecuted()
    {
        var wnd = _AppWindowsService.GetWindow<KeyCommandsWatcher>();
        wnd.Show();
    }

    #endregion

}