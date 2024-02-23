using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            //InitializeListener();
    }

    private void InitializeListener()
    {
        var radioSettings = _DataManager.RadioSettings;
        if(!radioSettings.IsEnabled || !radioSettings.Sources.Any())
            return;

        var loaded = _AudioPlayerService.LoadSources(radioSettings.Sources);
        if (loaded < radioSettings.Sources.Count)
        {
            AppLog.LogMessage($"Загружено источников потокового радио: {loaded} из {radioSettings.Sources.Count}", LogMessage.MessageType.Warning);
            if (loaded == 0) return;
        }

        var bindings = new List<JoyBindingBase>();

        if (radioSettings.PlayStopBinding != null)
            bindings.Add(radioSettings.PlayStopBinding);

        _JoyBindingListener.ChangesHandled += ChangesHandled;
        _JoyBindingListener.StartListen(bindings);
    }

    private void ChangesHandled(IEnumerable<JoyBindingBase> obj)
    {
        foreach (var joyBindingBase in obj)
        {
            Debug.WriteLine(joyBindingBase.IsActive);
        }

    }


    #region Command StartPlayCommand - Начать воспроизведение

    /// <summary>Начать воспроизведение</summary>
    private Command _StartPlayCommand;

    /// <summary>Начать воспроизведение</summary>
    public Command StartPlayCommand => _StartPlayCommand
        ??= new Command(OnStartPlayCommandExecuted, CanStartPlayCommandExecute, "Начать воспроизведение");

    /// <summary>Проверка возможности выполнения - Начать воспроизведение</summary>
    private bool CanStartPlayCommandExecute() => true;

    /// <summary>Логика выполнения - Начать воспроизведение</summary>
    private void OnStartPlayCommandExecuted()
    {
        InitializeListener();
    }

    #endregion

    public void Dispose()
    {
        _JoyBindingListener.StopListen();
        _JoyBindingListener.ChangesHandled -= ChangesHandled;
    }
}