using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Models.Base;
using JoyMapper.Models.Listeners;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class JoyBindingListener : IJoyBindingListener
{
    private readonly IJoystickStateManager _JoystickStateManager;
    private readonly DataManager _DataManager;
    private readonly int _PollingDelay;

    private List<ModificatedJoyBinding> _Bindings = new(); // Отслеживаемые привязки
    private List<Modificator> _Modificators = new(); // Используемые модификаторы
    private List<string> _UsedJoysticks = new(); // Используемые джойстики для модификаторов и привязок

    public Action<IEnumerable<JoyBindingBase>> ChangesHandled { get; set; }

    private CancellationTokenSource _CancellationTokenSource;


    public JoyBindingListener(IJoystickStateManager JoystickStateManager, DataManager DataManager)
    {
        _JoystickStateManager = JoystickStateManager;
        _DataManager = DataManager;
        _PollingDelay = DataManager.AppSettings.JoystickPollingDelay;
    }


    public void StartListen(IEnumerable<JoyBindingBase> bindings)
    {
        StartListen(bindings
            .Select(b => new ModificatedJoyBinding(b, 0, Array.Empty<int>())));
    }

    public void StartListen(IEnumerable<ModificatedJoyBinding> bindings)
    {
        StopListen();

        _Bindings = bindings.ToList();

        var usedModificators = _Bindings
            .Select(b => b.ModificatorId)
            .SelectMany(id => _DataManager.Modificators
                .Where(m => m.Id == id))
            .ToList();
        _Modificators = usedModificators;

        var joys = usedModificators
            .Select(m => m.Binding.JoyName)
            .Union(_Bindings
                .Select(b => b.BindingBase.JoyName))
            .Distinct()
            .ToList();

        var avaliableJoys = _JoystickStateManager
            .GetConnectedJoysticks()
            .Count(d => joys.Contains(d));

        if (avaliableJoys == 0)
        {
            AppLog.LogMessage("Джойстики, используемые в этом профиле, не найдены. Подключите их и перезапустите профиль"
                , LogMessage.MessageType.Error);
            return;
        }

        if (joys.Count > avaliableJoys)
        {
            AppLog.LogMessage($"Найдено {avaliableJoys} джойстиков из задействованных в профиле: {joys.Count}.\n" +
                              $"Подключите нужные устройства для правильной работы профиля", LogMessage.MessageType.Warning);
        }

        _UsedJoysticks = joys;

        UpdateStatus();

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


    /// <summary> Синхронизировать состояния кнопок </summary>
    private void UpdateStatus()
    {
        foreach (var state in _UsedJoysticks
                     .Select(joy => _JoystickStateManager
                         .GetJoyState(joy))
                     .Where(state => state != null))
        {
            _Modificators.ForEach(m => m.Binding.UpdateIsActive(state));
            _Bindings.ForEach(binding => binding.BindingBase.UpdateIsActive(state));
        }
    }


    /// <summary> Получить изменения в статусах привязок кнопок </summary>
    private ICollection<JoyBindingBase> GetChanges()
    {
        if(_CancellationTokenSource == null || _CancellationTokenSource.IsCancellationRequested)
            return new List<JoyBindingBase>();

        var changes = new List<JoyBindingBase>();

        // Опрос джойстиков
        var joyStates = _UsedJoysticks
            .Select(joy => new { name = joy, state = _JoystickStateManager.GetJoyState(joy) })
            .Where(js => js.state != null)
            .ToList();

        // Опрос модификаторов
        _Modificators.ForEach(m =>
        {
            var joyState = joyStates.Find(js => js.name.Equals(m.Binding.JoyName))?.state;
            if (joyState != null)
                m.Binding.UpdateIsActive(joyState);
        });

        // Опрос привязок
        foreach (var joyBinding in _Bindings)
        {
            var joyState = joyStates.Find(js => js.name.Equals(joyBinding.BindingBase.JoyName))?.state;
            if (joyState is null) continue;

            var lastState = joyBinding.BindingBase.IsActive;
            var newState = joyBinding.BindingBase.UpdateIsActive(joyState);

            if(joyBinding.ForbiddenExecuteModificatorsIds
               .Any(id => _Modificators
                   .Where(m=> m.Binding.IsActive)
                   .Select(m=> m.Id)
                   .Contains(id)))
                continue;

            if (joyBinding.ModificatorId > 0 && !_Modificators.First(m => m.Id == joyBinding.ModificatorId).Binding.IsActive)
                continue;

            if (lastState != newState)
                changes.Add(joyBinding.BindingBase);
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
        _Bindings.Clear();
        _Modificators.Clear();
        _UsedJoysticks.Clear();
        _CancellationTokenSource.Dispose();
        _CancellationTokenSource = null;
    }


}