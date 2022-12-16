using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Helpers;
using JoyMapper.Models;
using JoyMapper.Services.Interfaces;
using SharpDX.DirectInput;

namespace JoyMapper.Services;

public class JoystickStateManager : IJoystickStateManager, IDisposable
{
    public IEnumerable<string> GetConnectedJoysticks()
    {
        var connectedDevices = new DirectInput()
            .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

        return connectedDevices.Select(cd => cd.InstanceName);
    }

    private readonly List<Joystick> _Joysticks = new();

    public JoyState GetJoyState(string joystickName)
    {
        try
        {
            Joystick joystick;
            if (_Joysticks.Find(j => j.Information.InstanceName == joystickName) is { } joy)
            {
                joystick = joy;
            }
            else
            {
                var deviceInstance = new DirectInput()
                    .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    .FirstOrDefault(d => d.InstanceName == joystickName);

                if (deviceInstance == null)
                    return null;
                joystick = new Joystick(new DirectInput(), deviceInstance.InstanceGuid);
                _Joysticks.Add(joystick);
                joystick.Acquire();
            }

            
            //joystick.Poll();
            var joyState = joystick.GetCurrentState();

            return joyState.ToModel();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            AppLog.LogMessage($"Ошибка опроса устройства - {joystickName}", LogMessage.MessageType.Error);
            return null;
        }
    }

    public void Dispose()
    {
        foreach (var joystick in _Joysticks)
            joystick.Dispose();

        _Joysticks.Clear();
    }
}