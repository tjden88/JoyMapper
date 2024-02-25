using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        var joy = _Joysticks.Find(j => j.JoyName == joystickName);
        try
        {

            if (joy is null || joy.IsFault) // Была ошибка или джойстик не найден
            {
                var newJoy = new DirectInput()
                    .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    .FirstOrDefault(d => d.InstanceName == joystickName);

                if (newJoy == null)
                {
                    if (joy is null)
                    {
                        _Joysticks.Add(new ConnentedJoystick(joystickName) {IsFault = true});
                        AppLog.LogMessage($"Устройство {joystickName} не найдено!", LogMessage.MessageType.Error);
                    }

                    return null;
                }

                // Джойстик найден или заново подключен
                var newJoystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);

                if (joy is null) // Добавить
                {
                    joy = new(joystickName) { Joystick = newJoystick };
                    _Joysticks.Add(joy);
                }
                else // Заменить
                {
                    joy.Joystick?.Unacquire();
                    joy.Joystick?.Dispose();
                    joy.Joystick = newJoystick;
                }

                newJoystick.Properties.BufferSize = 128;
                newJoystick.Acquire();
            }


            if (joy.IsFault) 
                AppLog.LogMessage($"Устройство восстановлено - {joystickName}");
            joy.IsFault = false;


            // Джойстик есть в списке

            return joy.JoyState;

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
        public ConnentedJoystick(string Name)
        {
            JoyName = Name;
        }

        public bool IsFault { get; set; }

        public string JoyName { get; }

        public Joystick Joystick { get; set; }


        private JoyState _JoyState;

        public JoyState JoyState => _JoyState ??= UpdateState();

        private JoyState UpdateState()
        {
            if (Joystick is null)
                return null;

            Joystick.Poll();
            var datas = Joystick.GetBufferedData();
            var newState = _JoyState ?? new JoyState();

            foreach (var state in datas)
            {

            }

            return newState;
        }
    }
}