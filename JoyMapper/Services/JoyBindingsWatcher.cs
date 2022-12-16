using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services;

public class JoyBindingsWatcher : IJoyBindingsWatcher
{
    private readonly IJoystickStateManager _JoystickStateManager;

    private List<JoyBindingsGroup> _BindingsGroups; // Привязки кнопок по имени джойстика

    public JoyBindingsWatcher(IJoystickStateManager JoystickStateManager)
    {
        _JoystickStateManager = JoystickStateManager;
    }


    public void StartWatching(IEnumerable<JoyBindingBase> bindings)
    {
        var joyGroups = bindings
            .GroupBy(b => b.JoyName)
            .Select(g => new JoyBindingsGroup(g.Key, g.Select(gg=> (false, gg)).ToList() ))
            .ToList();
        _BindingsGroups = joyGroups;

        SyncState();
    }

    public IEnumerable<JoyBindingBase> GetChanges()
    {
        var changes = new List<JoyBindingBase>();
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null)
            {
                joyBindingsGroup.BindingStates.ForEach(gr =>
                {
                    var newState = gr.Binding.IsActive(state);
                    if (gr is {LastState: true} != newState)
                    {
                        gr.LastState = newState;
                        changes.Add(gr.Binding);
                    }
                });
            }
        }
        return changes;
    }

    public void StopWatching()
    {
        
    }


    private void SyncState()
    {
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null)
            {
                joyBindingsGroup.BindingStates.ForEach(gr =>
                {
                    gr.LastState = gr.Binding.IsActive(state);
                });
            }
        }
    }


    private record JoyBindingsGroup(string JoyName, List<(bool LastState, JoyBindingBase Binding)> BindingStates);
}