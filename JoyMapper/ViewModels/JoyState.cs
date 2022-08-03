using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Services;
using SharpDX.DirectInput;

namespace JoyMapper.ViewModels
{
    /// <summary>
    /// Текущий статус отслеживаемого джойстика
    /// </summary>
    internal class JoyState
    {
        private bool _IsFault; // Ошибка в опросе

        /// <summary> Статус действия </summary>
        public record ActionState(JoyActionOld ActionOld)
        {

            public bool IsActive { get; set; }

            public List<KeyboardKeyBinding> PressKeyBindings { get; set; }
            public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

        }

        private string _JoystickName;
        private Joystick _Joystick;
        /// <summary> Привязанный джойстик </summary>
        public Joystick Joystick
        {
            get => _Joystick;
            set
            {
                _Joystick = value;
                _Joystick.Acquire();
                _JoystickName = value?.Information.InstanceName;
            }
        }

        /// <summary> Статус используемых действий </summary>
        public List<ActionState> Actions { get; set; } = new();


        /// <summary>
        /// Получить различия нажатия используемых между прошлым и текущим состоянием джойстика.
        /// Сохраняет новое состояние
        /// </summary>
        /// <returns></returns>
        public List<ActionState> GetDifferents()
        {
            var result = new List<ActionState>();
            try
            {
                if (_IsFault) // была ошибка, пробуем переподключиться к джойстику
                {
                    var newJoy = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d => d.InstanceName == _JoystickName);
                    if (newJoy != null)
                    {
                        _IsFault = false;
                        Joystick.Dispose();
                        Joystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);
                        AppLog.LogMessage($"Устройство восстановлено - {_JoystickName}");
                    }
                    else
                        return result;
                }

                var joy = Joystick;
                var state = joy.GetCurrentState();


                foreach (var actionState in Actions)
                {
                    var isActiveNow = actionState.ActionOld.IsActionActive(ref state);
                    if (isActiveNow != actionState.IsActive)
                    {
                        result.Add(actionState);
                        actionState.IsActive = isActiveNow;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                AppLog.LogMessage($"Ошибка опроса устройства - {_JoystickName}", LogMessage.MessageType.Error);
                _IsFault = true;
            }

            return result;
        }

        /// <summary> Синхронизировать состояние </summary>
        public void SyncStatus()
        {
            try
            {
                var joy = Joystick;
                joy.Poll();
                var state = joy.GetCurrentState();
                foreach (var actionState in Actions)
                    actionState.IsActive = actionState.ActionOld.IsActionActive(ref state);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
