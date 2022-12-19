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

    private readonly List<ConnentedJoystick> _Joysticks = new();

    public JoyState GetJoyState(string joystickName)
    {
        var joy = _Joysticks.Find(j => j.Joystick?.Information.InstanceName == joystickName);
        try
        {

            if (joy is null || joy.IsFault) // Была ошибка или джойстик не найден
            {
                var newJoy = new DirectInput()
                    .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    .FirstOrDefault(d => d.InstanceName == joystickName);

                if (newJoy == null)
                    return null;

                // Джойстик найден или заново подключен
                var newJoystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);

                if (joy is null) // Добавить
                {
                    joy = new() { Joystick = newJoystick };
                    _Joysticks.Add(joy);
                }
                else // Заменить
                {
                    joy.Joystick?.Dispose();
                    joy.Joystick = newJoystick;
                }
                joy.Joystick.Acquire();
            }


            if (joy.IsFault) AppLog.LogMessage($"Устройство восстановлено - {joystickName}");
            joy.IsFault = false;


            // Джойстик есть в списке
            var joyState = joy.Joystick.GetCurrentState();
            return joyState.ToModel();
        }
        catch (Exception e)
        {
            if (joy != null)
                joy.IsFault = true;
            Debug.WriteLine(e);
            AppLog.LogMessage($"Ошибка опроса устройства - {joystickName}", LogMessage.MessageType.Error);
            return null;
        }
    }

    public void Dispose()
    {
        foreach (var joystick in _Joysticks)
            joystick.Joystick.Dispose();

        _Joysticks.Clear();
    }

    private class ConnentedJoystick
    {
        public bool IsFault { get; set; }

        public Joystick Joystick { get; set; }
    }
}