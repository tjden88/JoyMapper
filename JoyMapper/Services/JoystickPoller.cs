using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Helpers;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;
using JoyMapper.Services.ActionWatchers;
using SharpDX.DirectInput;

namespace JoyMapper.Services
{
    /// <summary>
    /// Опрашивает джойстик и передаёт клавиатурные команды
    /// </summary>
    internal class JoystickPoller
    {
        private record ActionCurrentState(ActionWatcherBase Watcher)
        {
            public bool IsActive { get; set; }
        }


        private readonly string _JoystickName;

        private bool _IsFault; // Ошибка в опросе


        private Joystick _Joystick;

        private readonly List<ActionCurrentState> _ActionsCurrentStates;

        public JoystickPoller(string JoystickName, IEnumerable<JoyActionBase> joyActions)
        {
            _JoystickName = JoystickName;
            _ActionsCurrentStates = joyActions
                .Select(act => new ActionCurrentState(ActionWatcherFactory.CreateActionWatcherBase(act)))
                .ToList();
        }

        /// <summary> Получить активные действия, которые изменили своё состояние с предыдущего опроса </summary>
        public List<JoyActionBase> GetActiveDifferents()
        {
            var result = new List<JoyActionBase>();

            var joyState = GetJoyState();

            if (joyState == null)
                return result;


            foreach (var state in _ActionsCurrentStates)
            {
                state.Watcher.Poll(joyState, false);
                if (state.IsActive != state.Watcher.IsActive)
                {
                    state.IsActive = state.Watcher.IsActive;
                    result.Add(state.Watcher.JoyAction);
                }
            }

            return result;
        }

        public void SyncActions()
        {
            var joyState = GetJoyState();

            if (joyState == null) return;

            foreach (var state in _ActionsCurrentStates)
            {
                state.Watcher.Poll(joyState, false);
                state.IsActive = state.Watcher.IsActive;
            }
        }


        private JoyState GetJoyState()
        {
            try
            {
                if (_IsFault || _Joystick == null) // была ошибка, пробуем переподключиться к джойстику
                {
                    var newJoy = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d => d.InstanceName == _JoystickName);

                    if (newJoy != null)
                    {
                        _IsFault = false;
                        _Joystick?.Dispose();
                        _Joystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);
                        AppLog.LogMessage($"Устройство восстановлено - {_JoystickName}");
                    }
                    else
                    {
                        return null;
                    }
                }

                var joy = _Joystick;

                return joy.GetCurrentState().ToModel();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                AppLog.LogMessage($"Ошибка опроса устройства - {_JoystickName}", LogMessage.MessageType.Error);
                _IsFault = true;
                return null;
            }
        }
    }
}

