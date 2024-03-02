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
    private readonly List<JoystickStateWatcher> _Joysticks = new();

    private static readonly DirectInput _DirectInput = new();

    private bool _IsInAcqired;

    public IEnumerable<string> GetConnectedJoysticks()
    {
        var connectedDevices = _DirectInput
            .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

        return connectedDevices.Select(cd => cd.InstanceName);
    }

    public void AcquireJoysticks(IEnumerable<string> JoysticksNames)
    {
        _IsInAcqired = true;
        var names = JoysticksNames.ToArray();

        var toRemove = _Joysticks.Where(watcher => !names.Contains(watcher.JoyName)).ToArray();
        foreach (var watcher in toRemove)
        {
            watcher.Dispose();
            _Joysticks.Remove(watcher);
            Debug.WriteLine($"Прекращено отслеживание состояний джойстика: {watcher.JoyName}");
        }


        foreach (var name in names)
        {
            if (_Joysticks.Any(j => j.JoyName.Equals(name)))
                continue;

            var newJoy = TryGetJoystick(name);
            if (newJoy == null) // Не найден
            {
                _Joysticks.Add(new JoystickStateWatcher(name) { IsFault = true });
                AppLog.LogMessage($"Устройство {name} не найдено!", LogMessage.MessageType.Error);
            }
            else // Найден
            {
                _Joysticks.Add(new JoystickStateWatcher(name)
                {
                    Joystick = newJoy
                });
                Debug.WriteLine($"Начато отслеживание состояний джойстика: {name}");
            }
        }

        _IsInAcqired = false;
    }

    public IEnumerable<JoyStateData> GetJoyStateChanges()
    {
        foreach (var joy in _Joysticks)
        {
            if (joy.IsFault)
            {
                var newjoy = TryGetJoystick(joy.JoyName);
                if (newjoy is not null)
                {
                    joy.Joystick = newjoy;
                    joy.IsFault = false;
                    AppLog.LogMessage($"Устройство восстановлено - {joy.JoyName}");
                    Debug.WriteLine($"Начато отслеживание состояний джойстика: {joy.JoyName}");
                }
                if (_IsInAcqired)
                    yield break;

                continue;
            }

            if(_IsInAcqired)
                yield break;

            foreach (var change in joy.GetChanges())
                yield return change;
        }
    }

    private static Joystick TryGetJoystick(string name)
    {
        var newJoy = _DirectInput
            .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
            .FirstOrDefault(d => d.InstanceName.Equals(name));
        return newJoy is null ? null : new Joystick(_DirectInput, newJoy.InstanceGuid);
    }

    public void Dispose()
    {
        foreach (var joystick in _Joysticks)
        {
            Debug.WriteLine($"Прекращение прослушивания джойстика {joystick.JoyName}");
            joystick.Dispose();
        }

        _Joysticks.Clear();
    }

    private class JoystickStateWatcher : IDisposable
    {
        private Joystick _Joystick;

        public JoystickStateWatcher(string Name)
        {
            JoyName = Name;
        }

        public bool IsFault { get; set; }

        public string JoyName { get; }

        public Joystick Joystick
        {
            get => _Joystick;
            set
            {
                if (_Joystick is { } oldJoy)
                {
                    oldJoy.Dispose();
                }

                if (value is { } newJoy)
                {
                    newJoy.Properties.BufferSize = 128;
                    newJoy.Acquire();
                    //newJoy.Poll();
                }

                _Joystick = value;
            }
        }

        /// <summary> Получить изменения состояний </summary>
        public IEnumerable<JoyStateData> GetChanges()
        {
            if (Joystick is null)
                return Enumerable.Empty<JoyStateData>();

            try
            {
                Joystick.Poll();
                var datas = Joystick.GetBufferedData();
                return GetData(datas);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                AppLog.LogMessage($"Ошибка опроса устройства - {JoyName}", LogMessage.MessageType.Error);
                IsFault = true;
                return Enumerable.Empty<JoyStateData>();
            }
        }

        private IEnumerable<JoyStateData> GetData(IEnumerable<JoystickUpdate> updates)
        {
            foreach (var update in updates)
            {
                var joyStateChange = SetJoyStateChange(update);
                if (joyStateChange != null)
                    yield return joyStateChange;
            }
        }


        private JoyStateData SetJoyStateChange(JoystickUpdate data)
        {
            // JoystickOffset.Buttons0 : 48
            // JoystickOffset.Buttons127 : 175
            if (data.RawOffset is >= 48 and <= 175)
            {
                return new JoyStateData(JoyName, data.RawOffset - 47, data.Value);
            }

            switch (data.Offset)
            {
                case JoystickOffset.PointOfViewControllers0:
                    return new JoyStateData(JoyName, 200, data.Value);
                case JoystickOffset.PointOfViewControllers1:
                    return new JoyStateData(JoyName, 201, data.Value);
                case JoystickOffset.X:
                    return new JoyStateData(JoyName, 300, data.Value);
                case JoystickOffset.Y:
                    return new JoyStateData(JoyName, 301, data.Value);
                case JoystickOffset.Z:
                    return new JoyStateData(JoyName, 302, data.Value);
                case JoystickOffset.RotationX:
                    return new JoyStateData(JoyName, 303, data.Value);
                case JoystickOffset.RotationY:
                    return new JoyStateData(JoyName, 304, data.Value);
                case JoystickOffset.RotationZ:
                    return new JoyStateData(JoyName, 305, data.Value);
                case JoystickOffset.Sliders0:
                    return new JoyStateData(JoyName, 306, data.Value);
                case JoystickOffset.Sliders1:
                    return new JoyStateData(JoyName, 307, data.Value);

                default:
                    Debug.WriteLine(data);
                    return null;
            }
        }

        public void Dispose()
        {
            Joystick = null;
        }
    }
}