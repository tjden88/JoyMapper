using System.Collections.Generic;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services;

public class JoyBindingsWatcher : IJoyBindingsWatcher
{
    public void StartWatching(IEnumerable<JoyBindingBase> bindings)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<JoyBindingBase> GetChanges()
    {
        throw new System.NotImplementedException();
    }

    public void StopWatching()
    {
        throw new System.NotImplementedException();
    }
}