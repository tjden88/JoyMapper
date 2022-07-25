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
        private Joystick _Joystick;
        private bool _IsFault; // Ошибка в опросе

        public record BtnState(int BtnNumber)
        {
            public bool IsPressed { get; set; }

            public List<KeyboardKeyBinding> PressKeyBindings { get; set; }
            public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

        }


        /// <summary> Привязанный джойстик </summary>
        public Joystick Joystick
        {
            get => _Joystick;
            set
            {
                _Joystick = value;
                _Joystick.Acquire();
            }
        }

        /// <summary> Статус используемых кнопок </summary>
        public List<BtnState> BtnStates { get; set; } = new();


        /// <summary>
        /// Получить различия нажатия используемых между прошлым и текущим состоянием джойстика.
        /// Сохраняет новое состояние
        /// </summary>
        /// <returns></returns>
        public List<BtnState> GetDifferents()
        {
            var result = new List<BtnState>();
            try
            {
                if (_IsFault) // была ошибка, пробуем переподключиться к джойстику
                {
                    var newJoy = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d => d.InstanceName == Joystick.Information.InstanceName);
                    if (newJoy != null)
                    {
                        _IsFault = false;
                        Joystick.Dispose();
                        Joystick = new Joystick(new DirectInput(), newJoy.InstanceGuid);
                        AppLog.LogMessage($"Устройство восстановлено - {Joystick.Information.InstanceName}");
                    }
                    else
                        return result;
                }

                var joy = Joystick;
                joy.Poll();
                var state = joy.GetCurrentState().Buttons;


                foreach (var btnState in BtnStates)
                {
                    var newBtnValue = state[btnState.BtnNumber - 1];
                    if (newBtnValue != btnState.IsPressed)
                    {
                        result.Add(btnState);
                        btnState.IsPressed = newBtnValue;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                AppLog.LogMessage($"Ошибка опроса устройства - {Joystick.Information.InstanceName}");
                _IsFault = true;
            }

            return result;
        }

        /// <summary> Синхронизировать состояние кнопок </summary>
        public void UpdateBtnStatus()
        {
            try
            {
                var joy = Joystick;
                joy.Poll();
                var state = joy.GetCurrentState().Buttons;
                foreach (var btnState in BtnStates)
                {
                    var newBtnValue = state[btnState.BtnNumber - 1];
                    btnState.IsPressed = newBtnValue;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
