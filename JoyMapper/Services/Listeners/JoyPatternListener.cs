using System;
using System.Collections.Generic;
using System.Linq;
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
                var forbidIds = p.ModificatorId is not null
                    ? Array.Empty<int>()
                    : Array.Empty<int>();
                return new ModificatedJoyBinding(new JoyBindingWithAction(p.Binding, p.PatternAction),
                        p.ModificatorId, forbidIds);
            });

        _JoyBindingListener.ChangesHandled += Listener_OnChangesHandled;
        _JoyBindingListener.StartListen(bindings);
    }

    private void Listener_OnChangesHandled(IEnumerable<JoyBindingBase> bindings)
    {
        foreach (var joyBindingBase in bindings)
        {
            var bb = (JoyBindingWithAction) joyBindingBase;
            bb.ActionBase.BindingStateChanged(bb.IsActive);
        }
    }

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
    }
}
