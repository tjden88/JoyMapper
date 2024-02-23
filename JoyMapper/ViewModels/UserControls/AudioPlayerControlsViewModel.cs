using System;
using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class AudioPlayerControlsViewModel : ViewModel, IDisposable
{
    private readonly IJoyBindingListener _JoyBindingListener;
    private readonly AudioPlayerService _AudioPlayerService;
    private readonly DataManager _DataManager;

    public AudioPlayerControlsViewModel(IJoyBindingListener joyBindingListener, AudioPlayerService audioPlayerService, DataManager dataManager)
    {
        _JoyBindingListener = joyBindingListener;
        _AudioPlayerService = audioPlayerService;
        _DataManager = dataManager;
        _JoyBindingListener.ChangesHandled += ChangesHandled;
        _AudioPlayerService.IsPlayingChanged += AudioPlayerServiceOnIsPlayingChanged;
        _AudioPlayerService.SourceChanged += AudioPlayerServiceOnSourceChanged;
        StartService();
    }

    private void AudioPlayerServiceOnSourceChanged(object sender, string e) => CurrentRadioDescription = e;

    private void AudioPlayerServiceOnIsPlayingChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPlaying));


    #region IsPlaying : bool - Играет ли радио

    /// <summary>Играет ли радио</summary>
    public bool IsPlaying
    {
        get => _AudioPlayerService.IsPlaying;
        private set
        {
            if(Equals(_AudioPlayerService.IsPlaying, value))
                return;
            if(value)
                _AudioPlayerService.Play();
            else
                _AudioPlayerService.Stop();
            OnPropertyChanged();
        }
    }

    #endregion


    #region CurrentRadioDescription : string - Текущий поток

    /// <summary>Текущий поток</summary>
    private string _CurrentRadioDescription;

    /// <summary>Текущий поток</summary>
    public string CurrentRadioDescription
    {
        get => _CurrentRadioDescription;
        private set => Set(ref _CurrentRadioDescription, value);
    }

    #endregion


    #region Volume : byte - Громкость

    /// <summary>Громкость</summary>
    public byte Volume
    {
        get => _AudioPlayerService.Volume;
        set
        {
            _AudioPlayerService.Volume = value;
            OnPropertyChanged();
        }
    }

    #endregion


    private void ChangesHandled(IEnumerable<JoyBindingBase> obj)
    {
        var radioSettings = _DataManager.RadioSettings;
        foreach (var joyBindingBase in obj)
        {
            if(!joyBindingBase.IsActive) continue;

            if ( Equals(joyBindingBase, radioSettings.PlayStopBinding))
                IsPlaying = !IsPlaying;

            if (Equals(joyBindingBase, radioSettings.NextBinding))
                _AudioPlayerService.Next();

            if (Equals(joyBindingBase, radioSettings.PreviousBinding))
                _AudioPlayerService.Previous();

            if (Equals(joyBindingBase, radioSettings.VolumeBinding))
            {
                var axisbb = (AxisJoyBinding) joyBindingBase;

                axisbb.EndValue = Math.Max(65536, axisbb.CurrentValue);
                axisbb.StartValue = Math.Min(0, axisbb.CurrentValue);
                var newVolume = (byte)(axisbb.CurrentValue / 128f);
                Volume = newVolume;
            }
        }

    }


    #region Command PlayStopCommand - Начать воспроизведение

    /// <summary>Начать воспроизведение</summary>
    private Command _PlayStopCommand;

    /// <summary>Начать воспроизведение</summary>
    public Command PlayStopCommand => _PlayStopCommand
        ??= new Command(OnPlayStopCommandExecuted, CanPlayStopCommandExecute, "Начать воспроизведение");

    /// <summary>Проверка возможности выполнения - Начать воспроизведение</summary>
    private bool CanPlayStopCommandExecute() => true;

    /// <summary>Логика выполнения - Начать воспроизведение</summary>
    private void OnPlayStopCommandExecuted()
    {
        IsPlaying = !IsPlaying;
    }

    #endregion

    #region Command NextCommand - Следующий

    /// <summary>Следующий</summary>
    private Command _NextCommand;

    /// <summary>Следующий</summary>
    public Command NextCommand => _NextCommand
        ??= new Command(OnNextCommandExecuted, CanNextCommandExecute, "Следующий");

    /// <summary>Проверка возможности выполнения - Следующий</summary>
    private bool CanNextCommandExecute() => true;

    /// <summary>Логика выполнения - Следующий</summary>
    private void OnNextCommandExecuted()
    {
        _AudioPlayerService.Next();
    }

    #endregion

    #region Command PreviousCommand - Предыдущий

    /// <summary>Предыдущий</summary>
    private Command _PreviousCommand;

    /// <summary>Предыдущий</summary>
    public Command PreviousCommand => _PreviousCommand
        ??= new Command(OnPreviousCommandExecuted, CanPreviousCommandExecute, "Предыдущий");

    /// <summary>Проверка возможности выполнения - Предыдущий</summary>
    private bool CanPreviousCommandExecute() => true;

    /// <summary>Логика выполнения - Предыдущий</summary>
    private void OnPreviousCommandExecuted()
    {
        _AudioPlayerService.Previous();
    }

    #endregion

    /// <summary>
    /// Начать прослушивание привязок и старт сервиса радио
    /// </summary>
    public void StartService()
    {
        var radioSettings = _DataManager.RadioSettings;
        if (!radioSettings.IsEnabled || !radioSettings.Sources.Any())
        {
            AppLog.LogMessage($"Радио: не настроено или выключено", LogMessage.MessageType.Error);
            return;
        }

        var loaded = _AudioPlayerService.LoadSources(radioSettings.Sources);
        if (loaded < radioSettings.Sources.Count)
        {
            AppLog.LogMessage($"Загружено источников потокового радио: {loaded} из {radioSettings.Sources.Count}", LogMessage.MessageType.Warning);
            if (loaded == 0) return;
        }

        var bindings = new List<JoyBindingBase>();

        if (radioSettings.PlayStopBinding != null)
            bindings.Add(radioSettings.PlayStopBinding);
        if (radioSettings.NextBinding != null)
            bindings.Add(radioSettings.NextBinding);
        if (radioSettings.PreviousBinding != null)
            bindings.Add(radioSettings.PreviousBinding);
        if (radioSettings.VolumeBinding is AxisJoyBinding volBinding)
        {
            volBinding.StartValue = 0;
            volBinding.EndValue = 65535;
            bindings.Add(volBinding);
        }

        _JoyBindingListener.StartListen(bindings);

    }


    /// <summary>
    /// Остановить воспроизведение и прослушивание привязок радио
    /// </summary>
    public void StopService()
    {
        _JoyBindingListener.StopListen();
    }


    public void Dispose()
    {
        _JoyBindingListener.StopListen();
        _JoyBindingListener.ChangesHandled -= ChangesHandled;
    }
}