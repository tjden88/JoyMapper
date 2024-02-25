﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Listeners;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class JoyPatternListener : IJoyPatternListener
{
    private readonly IJoyBindingListener _JoyBindingListener;
    private readonly IServiceProvider _ServiceProvider;

    public JoyPatternListener(IJoyBindingListener JoyBindingListener, IServiceProvider ServiceProvider)
    {
        _JoyBindingListener = JoyBindingListener;
        _ServiceProvider = ServiceProvider;
    }

    public void StartWatching(ICollection<JoyPattern> Patterns)
    {
        StopWatching();

        foreach (var joyPattern in Patterns)
        {
            joyPattern.PatternAction.Initialize(_ServiceProvider, false);
        }

        var bindings = Patterns
            .Select(p =>
            {
                var forbidIds = Patterns
                    .Where(ptrn => ptrn.Binding.Equals(p.Binding) && !ptrn.ModificatorId.Equals(p.ModificatorId))
                    .Select(ptrn => ptrn.ModificatorId);
                ;
                return new ModificatedJoyBinding(new JoyBindingWithAction(p.Binding, p.PatternAction),
                        p.ModificatorId, forbidIds.ToArray());
            });

        _JoyBindingListener.ChangesHandled += Listener_OnChangesHandled;
        _JoyBindingListener.StartListen(bindings);
    }

    private void Listener_OnChangesHandled(IEnumerable<JoyBindingBase> bindings)
    {
        foreach (var joyBindingBase in bindings)
        {
            var bb = (JoyBindingWithAction)joyBindingBase;
            //bb.ActionBase.BindingStateChanged(bb.IsActive);
            _ActionWorker.Add(bb.ActionBase, bb.IsActive);
        }
    }

    private readonly ActionWorker _ActionWorker = new();

    public void StopWatching()
    {
        _JoyBindingListener.ChangesHandled -= Listener_OnChangesHandled;
        _JoyBindingListener.StopListen();
    }


    private class JoyBindingWithAction : JoyBindingBase
    {
        private readonly JoyBindingBase _BindingBase;
        public PatternActionBase ActionBase { get; }

        public JoyBindingWithAction(JoyBindingBase BindingBase, PatternActionBase ActionBase)
        {
            _BindingBase = BindingBase;
            this.ActionBase = ActionBase;
            JoyName = BindingBase.JoyName;
        }

        protected override bool IsPressed(JoyState joyState) => _BindingBase.UpdateIsActive(joyState);

        public override string Description => _BindingBase.Description;
        public override bool Equals(JoyBindingBase other) => _BindingBase.Equals(other);
    }

    private class ActionWorker
    {
        private readonly ConcurrentQueue<(PatternActionBase, bool)> _Queue = new();

        private bool _TaskRunning;

        public async void Add(PatternActionBase action, bool newState)
        {
            _Queue.Enqueue((action, newState));

            if(!_TaskRunning)
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
