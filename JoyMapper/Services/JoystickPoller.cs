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


        public string JoystickName { get; init; }

        private bool _IsFault; // Ошибка в опросе


        private Joystick _Joystick;

        private readonly List<ActionCurrentState> _ActionsCurrentStates;

        public JoystickPoller(string JoystickName, IEnumerable<JoyActionBase> joyActions)
        {
            this.JoystickName = JoystickName;
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
                    if (state.IsActive) result.Add(state.Watcher.JoyAction);
                }
            }

            return result;
        }


        /// <summary> Синхронизировать текущее состояние действий и статуса джойстика </summary>
        public void SyncActions()
        {
            var joyState = GetJoyState(true);

            if (joyState == null) return;

            foreach (var state in _ActionsCurrentStates)
            {
                state.Watcher.Poll(joyState, false);
                state.IsActive = state.Watcher.IsActive;
            }
        }


        /// <summary> Опросить действия и отправить команды клавиатуры </summary>
        public void Poll()
        {
            var joyState = GetJoyState();

            if (joyState == null) return;

            foreach (var state in _ActionsCurrentStates)
            {
                state.Watcher.Poll(joyState, true);
            }
        }

        private JoyState GetJoyState(bool poll = false)
        {
            try
            {
                if (_IsFault || _Joystick == null) // была ошибка, пробуем переподключиться к джойстику
                {
                    var newJoy = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d => d.InstanceName == JoystickName);

                    if (newJoy != null)
                    {
                        _Joystick?.Dispose();
                        _Joystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);
                        _Joystick.Acquire();

                        if (_IsFault) AppLog.LogMessage($"Устройство восстановлено - {JoystickName}");
                        _IsFault = false;
                    }
                    else
                    {
                        return null;
                    }
                }

                var joy = _Joystick;
                if (poll) joy.Poll();
                Debug.WriteLine(joy.Information.InstanceName + " - state getted");
                return joy.GetCurrentState().ToModel();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                AppLog.LogMessage($"Ошибка опроса устройства - {JoystickName}", LogMessage.MessageType.Error);
                _IsFault = true;
                return null;
            }
        }
    }
}

