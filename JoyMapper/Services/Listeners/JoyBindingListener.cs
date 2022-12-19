using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Listeners;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class JoyBindingListener : IJoyBindingListener
{
    private readonly IJoystickStateManager _JoystickStateManager;
    private readonly int _PollingDelay;

    private List<JoyBindingsGroup> _BindingsGroups = new(); // Привязки кнопок по имени джойстика


    public Action<IEnumerable<JoyBindingBase>> ChangesHandled { get; set; }

    private CancellationTokenSource _CancellationTokenSource;


    public JoyBindingListener(IJoystickStateManager JoystickStateManager, DataManager DataManager)
    {
        _JoystickStateManager = JoystickStateManager;
        _PollingDelay = DataManager.AppSettings.JoystickPollingDelay;
    }


    public void StartListen(IEnumerable<JoyBindingBase> bindings)
    {
        StartListen(bindings.Select(b=>new ModificatedJoyBinding
        {
            BindingBase = b
        }));
    }

    public void StartListen(IEnumerable<ModificatedJoyBinding> bindings)
    {
        StopListen();

        var joyGroups = bindings
            .GroupBy(b => b.BindingBase.JoyName)
            .Select(g => new JoyBindingsGroup(g.Key, g.ToList()))
            .Where(g => !string.IsNullOrEmpty(g.JoyName))
            .ToList();
        _BindingsGroups = joyGroups;

        UpdateStatus();

        var cancel = new CancellationTokenSource();

        Task.Run(() => Polling(cancel.Token), cancel.Token);

        _CancellationTokenSource = cancel;

        Debug.WriteLine($"Начато прослушивание {joyGroups.Sum(b => b.Bindings.Count)} привязок");

    }

    public void StopListen()
    {
        if (_CancellationTokenSource == null) return;

        _CancellationTokenSource.Cancel();
        _BindingsGroups.Clear();
        _CancellationTokenSource = null;
        Debug.WriteLine("Прослушивание привязок остановлено");
    }


    /// <summary> Синхронизировать состояния кнопок </summary>
    private void UpdateStatus()
    {
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null)
                joyBindingsGroup.Bindings.ForEach(binding => binding.BindingBase.UpdateIsActive(state));
        }
    }


    /// <summary> Получить изменения в статусах привязок кнопок </summary>
    private ICollection<JoyBindingBase> GetChanges()
    {
        var changes = new List<JoyBindingBase>();
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null)
            {
                joyBindingsGroup.Bindings.ForEach(binding =>
                {
                    var lastState = binding.BindingBase.IsActive;
                    var newState = binding.BindingBase.UpdateIsActive(state);
                    if (lastState != newState)
                        changes.Add(binding.BindingBase);
                });
            }
            else
            {
                AppLog.LogMessage($"Ошибка опроса джойстика {joyBindingsGroup.JoyName}", LogMessage.MessageType.Error);
            }
        }
        return changes;
    }


    /// <summary> Цикл опроса джойстиков </summary>
    private async Task Polling(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            var changes = GetChanges();
            if (changes.Any())
                ChangesHandled?.Invoke(changes);

            await Task.Delay(_PollingDelay, cancel);
        }
    }


    /// <summary> Группа событий по имени джойстика </summary>
    private record JoyBindingsGroup(string JoyName, List<ModificatedJoyBinding> Bindings);

}