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
            .Select(g => new JoyBindingsGroup(g.Key, g.ToList() ))
            .Where(g=>!string.IsNullOrEmpty(g.JoyName))
            .ToList();
        _BindingsGroups = joyGroups;

        UpdateStatus();
    }

    public void UpdateStatus()
    {
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null) 
                joyBindingsGroup.Bindings.ForEach(binding => binding.UpdateIsActive(state));
        }
    }

    public ICollection<JoyBindingBase> GetChanges()
    {
        var changes = new List<JoyBindingBase>();
        foreach (var joyBindingsGroup in _BindingsGroups)
        {
            var state = _JoystickStateManager.GetJoyState(joyBindingsGroup.JoyName);
            if (state != null)
            {
                joyBindingsGroup.Bindings.ForEach(binding =>
                {
                    var lastState = binding.IsActive;
                    var newState = binding.UpdateIsActive(state);
                    if (lastState != newState) 
                        changes.Add(binding);
                });
            }
        }
        return changes;
    }

    public void StopWatching()
    {
        
    }

    
    private record JoyBindingsGroup(string JoyName, List<JoyBindingBase> Bindings);
}