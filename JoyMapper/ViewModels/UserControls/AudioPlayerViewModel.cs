using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.ViewModels.Windows;
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

    public AudioPlayerViewModel(AppWindowsService appWindowsService, AudioPlayerService audioPlayerService)
    {
        _AppWindowsService = appWindowsService;
        _AudioPlayerService = audioPlayerService;
    }


    #region Props

    #region AudioStreams : ObservableCollection<AudioSource> - Потоки воспроизведения

    /// <summary>Потоки воспроизведения</summary>
    private ObservableCollection<AudioSource> _AudioStreams = new();

    /// <summary>Потоки воспроизведения</summary>
    public ObservableCollection<AudioSource> AudioStreams
    {
        get => _AudioStreams;
        set => Set(ref _AudioStreams, value);
    }

    #endregion


    public IEnumerable<ConfigButtonSetup> ButtonsConfigs { get; } = new List<ConfigButtonSetup>
    {
        new("Старт/Стоп воспроизведения", false),
        new("Следующая радиостанция", false),
        new("Предыдущая радиостанция", false),
        new("Ось регулировки громкости", true),
    };

    #endregion


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
        var streamUrl = await WPRDialogHelper.InputTextAsync(App.ActiveWindow, "Укажите источник аудиопотока", "Локальный путь или URL-адрес аудиопотока");
        if (string.IsNullOrWhiteSpace(streamUrl))
            return;

        var source = new AudioSource(streamUrl);

        var isExist = _AudioStreams.Contains(source);
        if (isExist)
        {
            WPRDialogHelper.Bubble(App.ActiveWindow, "Источник уже существует", Background: StyleBrushes.DangerColorBrush);
            return;
        }

        if (await _AudioPlayerService.CheckAvaliable(streamUrl))
        {
            source.IsAvaliable = true;
            AudioStreams.Add(source);
        }
        else
            await WPRDialogHelper.ErrorAsync(App.ActiveWindow, "Источник недоступен");
    }

    #endregion


    #region Classes

    public class ConfigButtonSetup : ViewModel
    {
        public string Name { get; }
        public bool IsAxis { get; }

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
            set => Set(ref _BindingBase, value);
        }

        #endregion

    }

    public class AudioSource : ViewModel, IEquatable<AudioSource>
    {

        public AudioSource(string source) => _Source = source;


        #region Source : string - Источник

        /// <summary>Источник</summary>
        private string _Source;

        /// <summary>Источник</summary>
        public string Source
        {
            get => _Source;
            set => Set(ref _Source, value);
        }

        #endregion

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

        public bool Equals(AudioSource other) => other != null && _Source.Equals(other.Source);
    }

    public record AudioSourceStatus(PackIconKind Icon, Brush Foreground, string Description);

    #endregion
}