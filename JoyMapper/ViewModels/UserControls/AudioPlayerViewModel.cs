using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Interfaces;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Radio;
using JoyMapper.Services;
using JoyMapper.ViewModels.Windows;
using WPR.Dialogs;
using WPR.Domain.Models.Themes;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class AudioPlayerViewModel : ViewModel
{
    private readonly AppWindowsService _AppWindowsService;

    public AudioPlayerViewModel(AppWindowsService appWindowsService)
    {
        _AppWindowsService = appWindowsService;
    }


    #region Props

    #region AudioStreams : ObservableCollection<IAudioStream> - Потоки воспроизведения

    /// <summary>Потоки воспроизведения</summary>
    private ObservableCollection<IAudioStream> _AudioStreams = new();

    /// <summary>Потоки воспроизведения</summary>
    public ObservableCollection<IAudioStream> AudioStreams
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

        var stream = new AudioStreamFromUrl(streamUrl);
        if (await stream.IsAvaliable())
        {
            var isExist = _AudioStreams.Contains(stream);
            if (isExist)
            {
                WPRDialogHelper.Bubble(App.ActiveWindow, "Источник уже существует", Background: StyleBrushes.DangerColorBrush);
                return;
            }
            AudioStreams.Add(stream);
        }
        else
            await WPRDialogHelper.ErrorAsync(App.ActiveWindow, "Источник недоступен");
    }

    #endregion

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

}