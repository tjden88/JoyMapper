using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Helpers;
using JoyMapper.Models;
using JoyMapper.Services.Interfaces;
using SharpDX.DirectInput;

namespace JoyMapper.Services;

public class JoystickStateManager : IJoystickStateManager
{
    public IEnumerable<string> GetConnectedJoysticks()
    {
        var connectedDevices = new DirectInput()
            .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

        return connectedDevices.Select(cd => cd.InstanceName);
    }


    public JoyState GetJoyState(string joystickName)
    {
        try
        {
            var deviceInstance = new DirectInput()
                .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                .FirstOrDefault(d => d.InstanceName == joystickName);

            if (deviceInstance == null) 
                return null;

            using var joystick = new Joystick(new DirectInput(), deviceInstance.InstanceGuid);
            joystick.Acquire();
            joystick.Poll();
            return joystick.GetCurrentState().ToModel();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            AppLog.LogMessage($"Ошибка опроса устройства - {joystickName}", LogMessage.MessageType.Error);
            return null;
        }
    }
}