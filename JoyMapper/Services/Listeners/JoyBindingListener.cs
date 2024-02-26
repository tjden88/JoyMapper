using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class JoyBindingListener : IJoyBindingListener
{
    private readonly IJoystickStateManager _JoystickStateManager;

    private readonly int _PollingDelay;

    private CancellationTokenSource _CancellationTokenSource;

    public Action<JoyBindingBase> ChangesHandled { get; set; }

    private List<JoyBindingBase> _Bindings;

    public JoyBindingListener(IJoystickStateManager joystickStateManager, DataManager dataManager)
    {
        _JoystickStateManager = joystickStateManager;
        _PollingDelay = dataManager.AppSettings.JoystickPollingDelay;
    }

    public void StartListen(IEnumerable<JoyBindingBase> bindings)
    {
        StopListen();
        _Bindings = new(bindings);

        var usedJoysticks = _Bindings.Select(b => b.JoyName).Distinct();
        _JoystickStateManager.AcquireJoysticks(usedJoysticks);

        var cancel = new CancellationTokenSource();

        Task.Run(() => Polling(cancel.Token), cancel.Token).ConfigureAwait(false);

        _CancellationTokenSource = cancel;

        Debug.WriteLine($"Начато прослушивание {_Bindings.Count} привязок");

    }

    public void StopListen()
    {
        if (_CancellationTokenSource == null) return;

        _CancellationTokenSource.Cancel();

        Debug.WriteLine("Прослушивание привязок остановлено");
    }

    /// <summary> Цикл опроса джойстиков </summary>
    private async Task Polling(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            var changes = _JoystickStateManager.GetJoyStateChanges();
            foreach (var change in changes)
            {
                var changedBindings = _Bindings.Where(b => b.SetNewActiveStatus(change));
                foreach (var binding in changedBindings)
                    ChangesHandled?.Invoke(binding);

            }
            await Task.Delay(_PollingDelay, cancel);
        }
        _CancellationTokenSource.Dispose();
        _CancellationTokenSource = null;
    }
}