using System.Collections.Generic;
using SharpDX.DirectInput;

namespace JoyMapper.Models
{
    /// <summary>
    /// Текущий статус отслеживаемого джойстика
    /// </summary>
    internal class JoyState
    {
        private Joystick _Joystick;

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

            return result;
        }

        /// <summary> Синхронизировать состояние кнопок </summary>
        public void UpdateBtnStatus()
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
    }
}
