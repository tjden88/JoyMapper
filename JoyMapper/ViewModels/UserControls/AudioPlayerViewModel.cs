using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.Windows;
using NAudio.Wave;
using WPR.ColorTheme;
using WPR.Dialogs;
using WPR.Domain.Models.Themes;
using WPR.Icons;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class AudioPlayerViewModel : ViewModel
{
    private readonly AppWindowsService _AppWindowsService;
    private readonly AudioPlayerService _AudioPlayerService;
    private readonly DataManager _DataManager;

    public AudioPlayerViewModel(AppWindowsService appWindowsService, AudioPlayerService audioPlayerService, DataManager dataManager)
    {
        _AppWindowsService = appWindowsService;
        _AudioPlayerService = audioPlayerService;
        _DataManager = dataManager;
        LoadData();
    }


    #region Props

    #region AudioStreams : ObservableCollection<AudioSourceViewModel> - Потоки воспроизведения

    /// <summary>Потоки воспроизведения</summary>
    private ObservableCollection<AudioSourceViewModel> _AudioStreams = new();

    /// <summary>Потоки воспроизведения</summary>
    public ObservableCollection<AudioSourceViewModel> AudioStreams
    {
        get => _AudioStreams;
        set => Set(ref _AudioStreams, value);
    }

    #endregion


    #region SelectedSource : AudioSourceViewModel - Выбранный источник

    /// <summary>Выбранный источник</summary>
    private AudioSourceViewModel _SelectedSource;

    /// <summary>Выбранный источник</summary>
    public AudioSourceViewModel SelectedSource
    {
        get => _SelectedSource;
        set => Set(ref _SelectedSource, value);
    }

    #endregion


    #region IsEnabled : bool - Радио включено

    /// <summary>Радио включено</summary>
    private bool _IsEnabled;

    /// <summary>Радио включено</summary>
    public bool IsEnabled
    {
        get => _IsEnabled;
        set => IfSet(ref _IsEnabled, value).Then(b =>
        {
            _DataManager.RadioSettings.IsEnabled = b;
            _DataManager.SaveData();
        });
    }

    #endregion


    #region DeviceOutputs : ObservableCollection<AudioDeviceOutput> - Устройства воспроизведения

    /// <summary>Устройства воспроизведения</summary>
    private ObservableCollection<AudioDeviceOutput> _DeviceOutputs;

    /// <summary>Устройства воспроизведения</summary>
    public ObservableCollection<AudioDeviceOutput> DeviceOutputs
    {
        get => _DeviceOutputs;
        private set => Set(ref _DeviceOutputs, value);
    }

    #endregion


    #region SelectedDevice : AudioDeviceOutput - Выбранное устройство

    /// <summary>Выбранное устройство</summary>
    private AudioDeviceOutput _SelectedDevice;

    /// <summary>Выбранное устройство</summary>
    public AudioDeviceOutput SelectedDevice
    {
        get => _SelectedDevice;
        set => IfSet(ref _SelectedDevice, value)
            .Then(v =>
            {
                _DataManager.RadioSettings.OutputDeviceId = v?.Id;
                _DataManager.SaveData();
            });
    }

    #endregion

    

    /// <summary> Конфигурации управления радио </summary>
    public IEnumerable<ConfigButtonSetup> ButtonsConfigs { get; private set; }

    #endregion


    #region Commands


    #region Command SetBindingCommand : ConfigButtonSetup - Установить привязку кнопки

    /// <summary>Установить привязку кнопки</summary>
    private Command<ConfigButtonSetup> _SetBindingCommand;

    /// <summary>Установить привязку кнопки</summary>
    public Command<ConfigButtonSetup> SetBindingCommand => _SetBindingCommand
        ??= new Command<ConfigButtonSetup>(OnSetBindingCommandExecuted, CanSetBindingCommandExecute, "Установить привязку кнопки");

    /// <summary>Проверка возможности выполнения - Установить привязку кнопки</summary>
    private bool CanSetBindingCommandExecute(ConfigButtonSetup p) => true;

    /// <summary>Проверка возможности выполнения - Установить привязку кнопки</summary>
    private void OnSetBindingCommandExecuted(ConfigButtonSetup p)
    {
        var bind = _AppWindowsService.GetJoyBinding(p.IsAxis ? AddJoyBindingViewModel.BindingFilters.Axes : AddJoyBindingViewModel.BindingFilters.Buttons);
        if (bind is null) return;
        p.BindingBase = bind;
        p.WriteBindingToRadioSettings?.Invoke(bind);
    }

    #endregion

    #region Command ClearBindingCommand : ConfigButtonSetup - Очистить привязку кнопки

    /// <summary>Очистить привязку кнопки</summary>
    private Command<ConfigButtonSetup> _ClearBindingCommand;

    /// <summary>Очистить привязку кнопки</summary>
    public Command<ConfigButtonSetup> ClearBindingCommand => _ClearBindingCommand
        ??= new Command<ConfigButtonSetup>(OnClearBindingCommandExecuted, CanClearBindingCommandExecute, "Очистить привязку кнопки");

    /// <summary>Проверка возможности выполнения - Очистить привязку кнопки</summary>
    private bool CanClearBindingCommandExecute(ConfigButtonSetup p) => true;

    /// <summary>Проверка возможности выполнения - Очистить привязку кнопки</summary>
    private void OnClearBindingCommandExecuted(ConfigButtonSetup p)
    {
        p.BindingBase = null;
        p.WriteBindingToRadioSettings?.Invoke(null);
    }

    #endregion


    #region AsyncCommand AddStreamSourceCommandCommand - Добавить источник аудио

    /// <summary>Добавить источник аудио</summary>
    private AsyncCommand _AddStreamSourceCommandCommand;

    /// <summary>Добавить источник аудио</summary>
    public AsyncCommand AddStreamSourceCommand => _AddStreamSourceCommandCommand
        ??= new AsyncCommand(OnAddStreamSourceCommandCommandExecutedAsync, CanAddStreamSourceCommandCommandExecute, "Добавить источник аудио");

    /// <summary>Проверка возможности выполнения - Добавить источник аудио</summary>
    private bool CanAddStreamSourceCommandCommandExecute() => true;

    /// <summary>Логика выполнения - Добавить источник аудио</summary>
    private async Task OnAddStreamSourceCommandCommandExecutedAsync(CancellationToken cancel)
    {
        var source = await AddOrEditSource();
        if (source is null) return;
        AudioStreams.Add(source);
        _DataManager.RadioSettings.Sources.Add(source.Source);
        _DataManager.SaveData();
    }

    #endregion


    #region AsyncCommand EditSourceCommand - Редактировать источник

    /// <summary>Редактировать источник</summary>
    private AsyncCommand _EditSourceCommand;

    /// <summary>Редактировать источник</summary>
    public AsyncCommand EditSourceCommand => _EditSourceCommand
        ??= new AsyncCommand(OnEditSourceCommandExecutedAsync, CanEditSourceCommandExecute, "Редактировать источник");

    /// <summary>Проверка возможности выполнения - Редактировать источник</summary>
    private bool CanEditSourceCommandExecute() => SelectedSource != null;

    /// <summary>Логика выполнения - Редактировать источник</summary>
    private async Task OnEditSourceCommandExecutedAsync(CancellationToken cancel)
    {
        var originSource = SelectedSource;
        var source = await AddOrEditSource(originSource.Source);
        if (source is null) return;
        AudioStreams.Add(source);
        _DataManager.RadioSettings.Sources.Add(source.Source);
        _DataManager.SaveData();
    }

    #endregion


    #region AsyncCommand DeleteSourceCommand - Удалить источнник

    /// <summary>Удалить источнник</summary>
    private AsyncCommand _DeleteSourceCommand;

    /// <summary>Удалить источнник</summary>
    public AsyncCommand DeleteSourceCommand => _DeleteSourceCommand
        ??= new AsyncCommand(OnDeleteSourceCommandExecutedAsync, CanDeleteSourceCommandExecute, "Удалить источнник");

    /// <summary>Проверка возможности выполнения - Удалить источнник</summary>
    private bool CanDeleteSourceCommandExecute() => SelectedSource != null;

    /// <summary>Логика выполнения - Удалить источнник</summary>
    private async Task OnDeleteSourceCommandExecutedAsync(CancellationToken cancel)
    {
        var src = SelectedSource;

        var question = await WPRDialogHelper.QuestionAsync(App.ActiveWindow, $"Удалить источник {src.ShortSource}?");
        if(!question) return;

        AudioStreams.Remove(src);
        _DataManager.RadioSettings.Sources.Remove(src.Source);
        _DataManager.SaveData();
    }

    #endregion


    #region AsyncCommand RefreshSourcesCommand - Обновить статус источников аудио

    /// <summary>Обновить статус источников аудио</summary>
    private AsyncCommand _RefreshSourcesCommand;

    /// <summary>Обновить статус источников аудио</summary>
    public AsyncCommand RefreshSourcesCommand => _RefreshSourcesCommand
        ??= new AsyncCommand(OnRefreshSourcesCommandExecutedAsync, CanRefreshSourcesCommandExecute, "Проверить");

    /// <summary>Проверка возможности выполнения - Обновить статус источников аудио</summary>
    private bool CanRefreshSourcesCommandExecute() => _AudioStreams.Any();

    /// <summary>Логика выполнения - Обновить статус источников аудио</summary>
    private async Task OnRefreshSourcesCommandExecutedAsync(CancellationToken cancel)
    {
        RefreshSourcesCommand.Text = "Проверяется...";
        foreach (var source in _AudioStreams)
            source.IsAvaliable = await _AudioPlayerService.CheckAvaliable(source.Source).ConfigureAwait(false);

        RefreshSourcesCommand.Text = $"Проверено в {DateTime.Now.ToShortTimeString()}";
    }

    #endregion


    #region Command ClearSelectedDeviceCommand - Сбросить устройство вывода

    /// <summary>Сбросить устройство вывода</summary>
    private Command _ClearSelectedDeviceCommand;

    /// <summary>Сбросить устройство вывода</summary>
    public Command ClearSelectedDeviceCommand => _ClearSelectedDeviceCommand
        ??= new Command(OnClearSelectedDeviceCommandExecuted, CanClearSelectedDeviceCommandExecute, "Сбросить устройство вывода");

    /// <summary>Проверка возможности выполнения - Сбросить устройство вывода</summary>
    private bool CanClearSelectedDeviceCommandExecute() => SelectedDevice is not null;

    /// <summary>Логика выполнения - Сбросить устройство вывода</summary>
    private void OnClearSelectedDeviceCommandExecuted()
    {
        SelectedDevice = null;
    }

    #endregion

    #endregion



    private void LoadData()
    {
        var radioSettings = _DataManager.RadioSettings;
        _IsEnabled = radioSettings.IsEnabled;
        ButtonsConfigs = new List<ConfigButtonSetup>
        {
            new("Старт/Стоп воспроизведения", false) {BindingBase = radioSettings.PlayStopBinding, WriteBindingToRadioSettings = b =>
                {
                    _DataManager.RadioSettings.PlayStopBinding = b;
                    _DataManager.SaveData();
                }
            },
            new("Следующая радиостанция", false) {BindingBase = radioSettings.NextBinding, WriteBindingToRadioSettings = b =>
                {
                    _DataManager.RadioSettings.NextBinding = b;
                    _DataManager.SaveData();
                }
            },
            new("Предыдущая радиостанция", false) { BindingBase = radioSettings.PreviousBinding , WriteBindingToRadioSettings =
                b =>
                {
                    _DataManager.RadioSettings.PreviousBinding = b; 
                    _DataManager.SaveData(); 

                } },
            new("Ось регулировки громкости", true) {BindingBase = radioSettings.VolumeBinding, WriteBindingToRadioSettings = b =>
                {
                    _DataManager.RadioSettings.VolumeBinding = b;
                    _DataManager.SaveData();
                }
            },
        };
        AudioStreams = new(radioSettings.Sources.Select(s => new AudioSourceViewModel(s)));
        DeviceOutputs = new(DirectSoundOut.Devices.Select(d => new AudioDeviceOutput(d.Description, d.Guid)));
        if (radioSettings.OutputDeviceId is { } id) 
            _SelectedDevice = DeviceOutputs.FirstOrDefault(d => d.Id.Equals(id));
    }

    private async Task<AudioSourceViewModel> AddOrEditSource(string defaultSource = null)
    {
        var streamUrl = await WPRDialogHelper.InputTextAsync(App.ActiveWindow, "Укажите адрес источника аудиопотока", "Локальный путь или URL-адрес аудиопотока", defaultSource);
        if (string.IsNullOrWhiteSpace(streamUrl) || Equals(streamUrl, defaultSource))
            return null;

        var source = new AudioSourceViewModel(streamUrl);

        var isExist = _AudioStreams.Where(s => !Equals(s.Source, defaultSource)).Contains(source);
        if (isExist)
        {
            WPRDialogHelper.Bubble(App.ActiveWindow, "Источник уже существует", Background: StyleBrushes.DangerColorBrush);
            return null;
        }

        if (!await _AudioPlayerService.CheckAvaliable(streamUrl))
        {
            await WPRDialogHelper.ErrorAsync(App.ActiveWindow, "Источник недоступен");
            return null;
        }
        source.IsAvaliable = true;
        return source;
    }


    #region Classes

    public class ConfigButtonSetup : ViewModel
    {
        public string Name { get; }
        public bool IsAxis { get; }

        public Action<JoyBindingBase> WriteBindingToRadioSettings { get; init; }

        public ConfigButtonSetup(string Name, bool IsAxis)
        {
            this.Name = Name;
            this.IsAxis = IsAxis;
        }

        public string ButtonCaption => IsAxis ? "Назначить ось" : "Назначить кнопку";


        #region BindingBase : JoyBindingBase - Назанченная кнопка / ось

        /// <summary>Назанченная кнопка / ось</summary>
        private JoyBindingBase _BindingBase;

        /// <summary>Назанченная кнопка / ось</summary>
        public JoyBindingBase BindingBase
        {
            get => _BindingBase;
            set => IfSet(ref _BindingBase, value)
                .CallPropertyChanged(nameof(IsInverted));
        }

        #endregion

        #region IsInverted : bool - Привязка инвертирована

        /// <summary>Привязка инвертирована</summary>
        public bool IsInverted
        {
            get => BindingBase?.ActivationType == JoyBindingBase.ActivationTypes.Reverse;
            set
            {
                if(BindingBase is null) return;
                var newActivation = value ? JoyBindingBase.ActivationTypes.Reverse : JoyBindingBase.ActivationTypes.Normal;
                if(BindingBase.ActivationType == newActivation)
                    return;

                BindingBase.ActivationType = newActivation;
                OnPropertyChanged();
                WriteBindingToRadioSettings?.Invoke(BindingBase);
            }
        }

        #endregion

        

    }

    public class AudioSourceViewModel : ViewModel, IEquatable<AudioSourceViewModel>
    {

        public AudioSourceViewModel(string source) => _Source = source;


        #region Source : string - Источник

        /// <summary>Источник</summary>
        private string _Source;

        /// <summary>Источник</summary>
        public string Source
        {
            get => _Source;
            set => IfSet(ref _Source, value).CallPropertyChanged(nameof(ShortSource));
        }

        #endregion

        public string ShortSource => Source?.Length > 70 ? $"{Source[..70]}..." : Source;

        #region IsAvaliable : bool? - Доступность

        /// <summary>Доступность</summary>
        private bool? _IsAvaliable;


        /// <summary>Доступность</summary>
        public bool? IsAvaliable
        {
            get => _IsAvaliable;
            set => IfSet(ref _IsAvaliable, value)
                .CallPropertyChanged(nameof(Status));
        }

        #endregion

        /// <summary> Описание статуса доступности </summary>
        public AudioSourceStatus Status => IsAvaliable switch
        {
            false => new(PackIconKind.Error, StyleHelper.GetBrushFromResource(StyleBrushes.DangerColorBrush),
                "Источник недоступен"),
            true => new(PackIconKind.CheckBold, StyleHelper.GetBrushFromResource(StyleBrushes.SuccessColorBrush),
                "Доступен"),
            null => new(PackIconKind.CloudQuestionOutline, StyleHelper.GetBrushFromResource(StyleBrushes.SecondaryColorBrush), "Доступность источника не проверена")
        };

        public bool Equals(AudioSourceViewModel other) => other != null && _Source.Equals(other.Source);
    }

    public record AudioSourceStatus(PackIconKind Icon, Brush Foreground, string Description);

    public record AudioDeviceOutput(string Name, Guid Id);

    #endregion
}