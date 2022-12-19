using JoyMapper.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;

namespace JoyMapper.Services.Watchers;

public class JoyPatternWatcher : IJoyPatternWatcher
{
    private readonly IJoyListener _JoyListener;
    private readonly IServiceProvider _ServiceProvider;

    public JoyPatternWatcher(IJoyListener JoyListener, DataManager DataManager, IServiceProvider ServiceProvider)
    {
        _JoyListener = JoyListener;
        _ServiceProvider = ServiceProvider;
    }

    public void StartWatching(ICollection<JoyPattern> Patterns)
    {
        StopWatching();

        foreach (var joyPattern in Patterns)
        {
            joyPattern.PatternAction.Initialize(_ServiceProvider, false);
            joyPattern.PatternAction.ReportMessage += ReportPatternAction;
        }

        var bindings = Patterns.Select(p => new JoyBindingWithAction(p.Binding, p.PatternAction));

        _JoyListener.ChangesHandled += Listener_OnChangesHandled;
        _JoyListener.StartListen(bindings);
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
        _JoyListener.ChangesHandled -= Listener_OnChangesHandled;
        _JoyListener.StopListen();
    }

    public Action<string> ReportPatternAction { get; set; }

    private class JoyBindingWithAction : JoyBindingBase
    {
        private readonly JoyBindingBase _BindingBase;
        public PatternActionBase ActionBase { get; }

        public JoyBindingWithAction(JoyBindingBase BindingBase, PatternActionBase ActionBase)
        {
            _BindingBase = BindingBase;
            this.ActionBase = ActionBase;
        }

        protected override bool IsPressed(JoyState joyState) => _BindingBase.UpdateIsActive(joyState);

        public override string Description => _BindingBase.Description;
    }
}
