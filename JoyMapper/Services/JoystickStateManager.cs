using System.Collections.Generic;
using JoyMapper.Models;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services;

public class JoystickStateManager : IJoystickStateManager
{
    public IEnumerable<string> GetConnectedJoysticks()
    {
        throw new System.NotImplementedException();
    }

    public JoyState GetJoyState(string joystickName)
    {
        throw new System.NotImplementedException();
    }
}