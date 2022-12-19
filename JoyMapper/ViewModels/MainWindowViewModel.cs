using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels;

public partial class MainWindowViewModel : WindowViewModel
{
    private readonly DataManager _DataManager;
    private readonly UpdateChecker _UpdateChecker;
    private readonly IProfileListener _ProfileListener;

    public MainWindowViewModel(DataManager DataManager, UpdateChecker UpdateChecker , IProfileListener ProfileListener)
    {
        _DataManager = DataManager;
        _UpdateChecker = UpdateChecker;
        _ProfileListener = ProfileListener;
        AppLog.Report += msg => LogMessages.Add(msg);
        Title = "JoyMapper " + App.AppVersion;
        CurrentColorTheme = ColorThemes
                                .FirstOrDefault(c => c.Id == _DataManager.AppSettings.CurrentColorCheme)
                            ?? ColorThemes.First();
    }


    #region Props

    #region Profiles : ObservableCollection<Profile> - Список профилей

    /// <summary>Список профилей</summary>
    private ObservableCollection<Profile> _Profiles;

    /// <summary>Список профилей</summary>
    public ObservableCollection<Profile> Profiles
    {
        get => _Profiles;
        set => Set(ref _Profiles, value);
    }

    #endregion


    #region SelectedProfile : Profile - Выбранный профиль

    /// <summary>Выбранный профиль</summary>
    private Profile _SelectedProfile;

    /// <summary>Выбранный профиль</summary>
    public Profile SelectedProfile
    {
        get => _SelectedProfile;
        set => Set(ref _SelectedProfile, value);
    }

    #endregion


    #region JoyPatterns : ObservableCollection<JoyPattern> - Список паттернов

    /// <summary>Список паттернов</summary>
    private ObservableCollection<JoyPattern> _JoyPatterns;

    /// <summary>Список паттернов</summary>
    public ObservableCollection<JoyPattern> JoyPatterns
    {
        get => _JoyPatterns;
        set => Set(ref _JoyPatterns, value);
    }

    #endregion


    #region SelectedPattern : JoyPattern - Выбранный паттерн

    /// <summary>Выбранный паттерн</summary>
    private JoyPattern _SelectedPattern;

    /// <summary>Выбранный паттерн</summary>
    public JoyPattern SelectedPattern
    {
        get => _SelectedPattern;
        set => Set(ref _SelectedPattern, value);
    }

    #endregion


    #region Modificators : ObservableCollection<Modificator> - Список модификаторов

    /// <summary>Список модификаторов</summary>
    private ObservableCollection<Modificator> _Modificators;

    /// <summary>Список модификаторов</summary>
    public ObservableCollection<Modificator> Modificators
    {
        get => _Modificators;
        set => Set(ref _Modificators, value);
    }

    #endregion


    #region SelectedModificator : Modificator - Выбранный модификатор

    /// <summary>Выбранный модификатор</summary>
    private Modificator _SelectedModificator;

    /// <summary>Выбранный модификатор</summary>
    public Modificator SelectedModificator
    {
        get => _SelectedModificator;
        set => Set(ref _SelectedModificator, value);
    }

    #endregion


    #endregion


    #region Commands

    #region Command LoadDataCommand - Загрузить данные

    /// <summary>Загрузить данные</summary>
    private Command _LoadProfilesCommand;

    /// <summary>Загрузить данные</summary>
    public Command LoadDataCommand => _LoadProfilesCommand
        ??= new Command(OnLoadProfilesCommandExecuted, CanLoadProfilesCommandExecute, "Загрузить данные");

    /// <summary>Проверка возможности выполнения - Загрузить данные</summary>
    private bool CanLoadProfilesCommandExecute() => true;

    /// <summary>Логика выполнения - Загрузить данные</summary>
    private void OnLoadProfilesCommandExecuted()
    {
        Profiles = new(_DataManager.Profiles);
        JoyPatterns = new(_DataManager.JoyPatterns);
        Modificators = new(_DataManager.Modificators);

    }

    #endregion


    #region Command CreateProfileCommand - Создать профиль

    /// <summary>Создать профиль</summary>
    private Command _CreateProfileCommand;

    /// <summary>Создать профиль</summary>
    public Command CreateProfileCommand => _CreateProfileCommand
        ??= new Command(OnCreateProfileCommandExecuted, CanCreateProfileCommandExecute, "Создать новый профиль");

    /// <summary>Проверка возможности выполнения - Создать профиль</summary>
    private bool CanCreateProfileCommandExecute() => true;

    /// <summary>Логика выполнения - Создать профиль</summary>
    private void OnCreateProfileCommandExecuted()
    {
        var newProfile = _DataManager.AddProfile();
        if(newProfile == null)return;

        Profiles.Add(newProfile);
        SelectedProfile = newProfile;
    }

    #endregion


    #region Command CopyProfileCommand - Копировать профиль

    /// <summary>Копировать профиль</summary>
    private Command _CopyProfileCommand;

    /// <summary>Копировать профиль</summary>
    public Command CopyProfileCommand => _CopyProfileCommand
        ??= new Command(OnCopyProfileCommandExecuted, CanCopyProfileCommandExecute, "Копировать профиль");

    /// <summary>Проверка возможности выполнения - Копировать профиль</summary>
    private bool CanCopyProfileCommandExecute() => SelectedProfile != null;

    /// <summary>Логика выполнения - Копировать профиль</summary>
    private void OnCopyProfileCommandExecuted()
    {
        var prof = _DataManager.CopyProfile(SelectedProfile.Id);
        if(prof == null) return;
        Profiles.Add(prof);
        SelectedProfile = prof;
        WPRMessageBox.Bubble(App.ActiveWindow, "Профиль скопирован");
    }

    #endregion


    #region Command EditProfileCommand - Редактировать профиль

    /// <summary>Редактировать профиль</summary>
    private Command _EditProfileCommand;

    /// <summary>Редактировать профиль</summary>
    public Command EditProfileCommand => _EditProfileCommand
        ??= new Command(OnEditProfileCommandExecuted, CanEditProfileCommandExecute, "Редактировать профиль");

    /// <summary>Проверка возможности выполнения - Редактировать профиль</summary>
    private bool CanEditProfileCommandExecute() => SelectedProfile != null;

    /// <summary>Логика выполнения - Редактировать профиль</summary>
    private void OnEditProfileCommandExecuted()
    {
        var updated = _DataManager.UpdateProfile(SelectedProfile.Id);
        if(updated == null) return;

        var index = Profiles.IndexOf(SelectedProfile);

        Profiles.Remove(SelectedProfile);
        Profiles.Insert(index, updated);
        SelectedProfile = updated;
    }

    #endregion


    #region AsyncCommand DeleteProfileCommand - Удалить профиль

    /// <summary>Удалить профиль</summary>
    private AsyncCommand _DeleteProfileCommand;

    /// <summary>Удалить профиль</summary>
    public AsyncCommand DeleteProfileCommand => _DeleteProfileCommand
        ??= new AsyncCommand(OnDeleteProfileCommandExecutedAsync, CanDeleteProfileCommandExecute, "Удалить профиль");

    /// <summary>Проверка возможности выполнения - Удалить профиль</summary>
    private bool CanDeleteProfileCommandExecute() => SelectedProfile != null;

    /// <summary>Логика выполнения - Удалить профиль</summary>
    private async Task OnDeleteProfileCommandExecutedAsync(CancellationToken cancel)
    {
        var result = await WPRMessageBox.QuestionAsync(App.ActiveWindow, $"Удалить профиль {SelectedProfile.Name}?");
        if (result)
        {
            _DataManager.RemoveProfile(SelectedProfile.Id);
            Profiles.Remove(SelectedProfile);
            SelectedProfile = null;
        }
    }

    #endregion


    #region Command CreatePatternCommand - Создать паттерн

    /// <summary>Создать паттерн</summary>
    private Command _CreatePatternCommand;

    /// <summary>Создать паттерн</summary>
    public Command CreatePatternCommand => _CreatePatternCommand
        ??= new Command(OnCreatePatternCommandExecuted, CanCreatePatternCommandExecute, "Создать паттерн");

    /// <summary>Проверка возможности выполнения - Создать паттерн</summary>
    private bool CanCreatePatternCommandExecute() => true;

    /// <summary>Логика выполнения - Создать паттерн</summary>
    private void OnCreatePatternCommandExecuted()
    {
        var pattern = _DataManager.AddJoyPattern();
        if(pattern == null) return;

        JoyPatterns.Add(pattern);
        SelectedPattern = pattern;
    }

    #endregion


    #region Command EditPatternCommand - Редактировать паттерн

    /// <summary>Редактировать паттерн</summary>
    private Command _EditPatternCommand;

    /// <summary>Редактировать паттерн</summary>
    public Command EditPatternCommand => _EditPatternCommand
        ??= new Command(OnEditPatternCommandExecuted, CanEditPatternCommandExecute, "Редактировать паттерн");

    /// <summary>Проверка возможности выполнения - Редактировать паттерн</summary>
    private bool CanEditPatternCommandExecute() => SelectedPattern != null;

    /// <summary>Логика выполнения - Редактировать паттерн</summary>
    private void OnEditPatternCommandExecuted()
    {
        var pattern = _DataManager.UpdateJoyPattern(SelectedPattern.Id);
            
        if(pattern == null) return;

        var index = JoyPatterns.IndexOf(SelectedPattern);

        JoyPatterns.Remove(SelectedPattern);
        JoyPatterns.Insert(index, pattern);
        SelectedPattern = pattern;
    }

    #endregion


    #region Command CopyPatternCommand - Создать копию паттерна

    /// <summary>Создать копию паттерна</summary>
    private Command _CopyPatternCommand;

    /// <summary>Создать копию паттерна</summary>
    public Command CopyPatternCommand => _CopyPatternCommand
        ??= new Command(OnCopyPatternCommandExecuted, CanCopyPatternCommandExecute, "Создать копию паттерна");

    /// <summary>Проверка возможности выполнения - Создать копию паттерна</summary>
    private bool CanCopyPatternCommandExecute() => SelectedPattern != null;

    /// <summary>Логика выполнения - Создать копию паттерна</summary>
    private void OnCopyPatternCommandExecuted()
    {
        var newPattern = _DataManager.CopyJoyPattern(SelectedPattern.Id);
        JoyPatterns.Add(newPattern);
        SelectedPattern = newPattern;
        WPRMessageBox.Bubble(App.ActiveWindow, "Паттерн скопирован");
    }

    #endregion


    #region AsyncCommand DeletePatternCommand - Удалить паттерн

    /// <summary>Удалить паттерн</summary>
    private AsyncCommand _DeletePatternCommand;

    /// <summary>Удалить паттерн</summary>
    public AsyncCommand DeletePatternCommand => _DeletePatternCommand
        ??= new AsyncCommand(OnDeletePatternCommandExecutedAsync, CanDeletePatternCommandExecute, "Удалить паттерн");

    /// <summary>Проверка возможности выполнения - Удалить паттерн</summary>
    private bool CanDeletePatternCommandExecute() => SelectedPattern != null;

    /// <summary>Логика выполнения - Удалить паттерн</summary>
    private async Task OnDeletePatternCommandExecutedAsync(CancellationToken cancel)
    {
        var result = await WPRMessageBox.QuestionAsync(App.ActiveWindow, $"Удалить паттерн {SelectedPattern.Name}?");
        if (result)
        {
            _DataManager.RemoveJoyPattern(SelectedPattern.Id);
            JoyPatterns.Remove(SelectedPattern);
            SelectedPattern = null;
        }
    }

    #endregion


    #region Command CreateModificatorCommand - Создать модификатор

    /// <summary>Создать модификатор</summary>
    private Command _CreateModificatorCommand;

    /// <summary>Создать модификатор</summary>
    public Command CreateModificatorCommand => _CreateModificatorCommand
        ??= new Command(OnCreateModificatorCommandExecuted, CanCreateModificatorCommandExecute, "Создать новый модификатор");

    /// <summary>Проверка возможности выполнения - Создать модификатор</summary>
    private bool CanCreateModificatorCommandExecute() => true;

    /// <summary>Логика выполнения - Создать модификатор</summary>
    private void OnCreateModificatorCommandExecuted()
    {
        var modificator = _DataManager.AddModificator();

        if(modificator is null)
            return;

        Modificators.Add(modificator);
        SelectedModificator = modificator;

    }

    #endregion


    #region Command EditModificatorCommand - Редактировать модификатор

    /// <summary>Редактировать модификатор</summary>
    private Command _EditModificatorCommand;

    /// <summary>Редактировать модификатор</summary>
    public Command EditModificatorCommand => _EditModificatorCommand
        ??= new Command(OnEditModificatorCommandExecuted, CanEditModificatorCommandExecute, "Редактировать модификатор");

    /// <summary>Проверка возможности выполнения - Редактировать модификатор</summary>
    private bool CanEditModificatorCommandExecute() => SelectedModificator != null;

    /// <summary>Логика выполнения - Редактировать модификатор</summary>
    private void OnEditModificatorCommandExecuted()
    {
        var edited = _DataManager.UpdateModificator(SelectedModificator.Id);

        if(edited is null)
            return;

        var index = Modificators.IndexOf(SelectedModificator);

        Modificators.Remove(SelectedModificator);
        Modificators.Insert(index, edited);
        SelectedModificator = edited;
    }

    #endregion


    #region AsyncCommand DeleteModificatorCommand - Удалить модификатор

    /// <summary>Удалить модификатор</summary>
    private AsyncCommand _DeleteModificatorCommand;

    /// <summary>Удалить модификатор</summary>
    public AsyncCommand DeleteModificatorCommand => _DeleteModificatorCommand
        ??= new AsyncCommand(OnDeleteModificatorCommandExecutedAsync, CanDeleteModificatorCommandExecute, "Удалить выбранный модификатор");

    /// <summary>Проверка возможности выполнения - Удалить модификатор</summary>
    private bool CanDeleteModificatorCommandExecute() => SelectedModificator != null;

    /// <summary>Логика выполнения - Удалить модификатор</summary>
    private async Task OnDeleteModificatorCommandExecutedAsync(CancellationToken cancel)
    {
        var result = await WPRMessageBox.QuestionAsync(App.ActiveWindow, $"Удалить модификатор {SelectedModificator.Name}?");
        if (result)
        {
            _DataManager.RemoveModificator(SelectedModificator.Id);
            Modificators.Remove(SelectedModificator);
            SelectedModificator = null;
        }
    }

    #endregion


    #endregion

}