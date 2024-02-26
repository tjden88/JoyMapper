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

    private List<PatternModel> _Patterns;

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
        var usedModificatorsIds = Patterns
            .Where(p => p.HasModificator)
            .Select(p => p.ModificatorId)
            .Distinct();

        var modificators = _DataManager.Modificators
            .Where(m => usedModificatorsIds.Contains(m.Id))
            .ToList();


        _Patterns = new(Patterns
            .Select(p => new PatternModel(p, modificators
                .FirstOrDefault(m => m.Id == p.ModificatorId))));

        foreach (var joyPattern in Patterns)
            joyPattern.PatternAction.Initialize(_ServiceProvider, false);


        var bindings = Patterns
            .Select(p => p.Binding)
            .Concat(modificators.Select(m => m.Binding))
            .Distinct(new BindingComparer());

        _ActionWorker = new();
        _BindingListener.StartListen(bindings);
    }

    public void StopWatching()
    {
        _ActionWorker?.Dispose();
        _BindingListener.StopListen();
    }

    private void BindingChangesHandled(JoyBindingBase binding)
    {
        var patterns = _Patterns
            .Where(p => p.Equals(binding))
            .ToArray();

        var hasActiveModificator = patterns.Any(p => p.Modificator is { Binding.IsActive: true });

        foreach (var patternModel in patterns)
        {
            if (patternModel.Modificator == null && hasActiveModificator)
                continue;

            if (patternModel.CanStateChange)
                _ActionWorker.Add(patternModel.Pattern.PatternAction, binding.IsActive);
        }

    }

    private ActionWorker _ActionWorker;

    #region Classes

    private sealed record PatternModel(JoyPattern Pattern, Modificator Modificator) : IEquatable<JoyBindingBase>
    {
        public bool CanStateChange => Modificator?.Binding.IsActive ?? true;
        public bool Equals(JoyBindingBase other) => Pattern.Binding.Equals(other);
    }


    private class ActionWorker : IDisposable
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

        public void Dispose() => _Queue.Clear();
    }


    private class BindingComparer : IEqualityComparer<JoyBindingBase>
    {
        public bool Equals(JoyBindingBase x, JoyBindingBase y) => x?.Equals(y) ?? false;

        public int GetHashCode(JoyBindingBase obj) => obj.Description.GetHashCode();
    }

    #endregion
}


