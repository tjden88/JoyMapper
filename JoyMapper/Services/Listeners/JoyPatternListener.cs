using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class JoyPatternListener : IJoyPatternListener
{
    private readonly IJoyBindingListener _BindingListener;
    private readonly IServiceProvider _ServiceProvider;
    private readonly DataManager _DataManager;

    private List<JoyPattern> _Patterns;
    private List<Modificator> _Modificators;

    public JoyPatternListener(IJoyBindingListener bindingListener, IServiceProvider ServiceProvider, DataManager dataManager)
    {
        _BindingListener = bindingListener;
        _ServiceProvider = ServiceProvider;
        _DataManager = dataManager;
        bindingListener.ChangesHandled += BindingChangesHandled;
    }

    public void StartWatching(ICollection<JoyPattern> Patterns)
    {
        StopWatching();

        foreach (var joyPattern in Patterns) 
            joyPattern.PatternAction.Initialize(_ServiceProvider, false);

        _Patterns = new(Patterns);

        var usedModificatorsIds = Patterns
            .Where(p => p.HasModificator)
            .Select(p => p.ModificatorId)
            .Distinct();

        var modificators = _DataManager.Modificators.Where(m => usedModificatorsIds.Contains(m.Id));
        _Modificators = new(modificators);

        var bindings = Patterns
            .Select(p => p.Binding)
            .Concat(_Modificators.Select(m=>m.Binding))
            .Distinct();

        _BindingListener.StartListen(bindings);
    }

    public void StopWatching()
    {
        _BindingListener.StopListen();
    }

    private void BindingChangesHandled(JoyBindingBase binding)
    {
        var patterns = _Patterns.Where(p => p.Binding.Equals(binding));
        foreach (var joyPattern in patterns)
        {
            // TODO: модификаторы
            joyPattern.PatternAction.BindingStateChanged(binding.IsActive);
        }

    }

    private class ActionWorker
    {
        private readonly ConcurrentQueue<(PatternActionBase, bool)> _Queue = new();

        private bool _TaskRunning;

        public async void Add(PatternActionBase action, bool newState)
        {
            _Queue.Enqueue((action, newState));

            if (!_TaskRunning)
                await DoActions().ConfigureAwait(false);
        }

        private Task DoActions()
        {
            return Task.Run(() =>
            {
                _TaskRunning = true;
                while (!_Queue.IsEmpty)
                {
                    if (_Queue.TryDequeue(out var actionBase))
                        actionBase.Item1.BindingStateChanged(actionBase.Item2);
                }
                _TaskRunning = false;
            });
        }
    }
}
