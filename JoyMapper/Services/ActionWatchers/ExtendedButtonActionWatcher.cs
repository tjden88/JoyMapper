using System;
using System.Diagnostics;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers
{
    internal class ExtendedButtonActionWatcher : ActionWatcherBase
    {
        private readonly ExtendedButtonJoyAction _ButtonJoyAction;

        private readonly int _DoublePressDelay = 400;
        private readonly int _LongPressDelay = 500;

        private Stopwatch _DelayMeter; // Таймер задержки между нажатиями

        private bool _FirstPressHandled; // Первое нажатие поймано

        public override JoyActionBase JoyAction => _ButtonJoyAction;

        public ExtendedButtonActionWatcher(ExtendedButtonJoyAction buttonJoyAction)
        {
            _ButtonJoyAction = buttonJoyAction;
        }

        public override void Poll(JoyState joyState, bool SendCommands)
        {
            var btnState = _ButtonJoyAction.Button.Type switch
            {
                ButtonType.Button => joyState.Buttons[_ButtonJoyAction.Button.Value],
                ButtonType.Pow1 => joyState.Pow1Value == _ButtonJoyAction.Button.Value,
                ButtonType.Pow2 => joyState.Pow2Value == _ButtonJoyAction.Button.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

            var prewState = IsActive;
            IsActive = btnState;

            if(!SendCommands) return;

            // Кнопка не нажата и прошло время двойного клика
            if (_FirstPressHandled && !btnState && _DelayMeter?.ElapsedMilliseconds > _DoublePressDelay)
            {
                Debug.WriteLine("SinglePressSend: " + _ButtonJoyAction.Button);
                SendSinglePress();
                _FirstPressHandled = false;
                _DelayMeter = null;
            }
            

            if (!prewState && btnState) // Состояние изменилось на нажатое
            {
                if (!_FirstPressHandled) // Регистрируем первое нажатие
                {
                    _FirstPressHandled = true;
                    _DelayMeter = Stopwatch.StartNew();
                }
                else // Второе нажатие
                {
                    Debug.WriteLine("DoublePressSend: " + _ButtonJoyAction.Button);
                    SendDoublePress();
                    _FirstPressHandled = false;
                    _DelayMeter = null;
                }
            }

            // Кнопка нажата больше времени долгого нажатия
            if (_FirstPressHandled && btnState && _DelayMeter?.ElapsedMilliseconds > _LongPressDelay)
            {
                Debug.WriteLine("LongPressSend: " + _ButtonJoyAction.Button);
                SendLongPress();
                _FirstPressHandled = false;
                _DelayMeter = null;
            }


            IsActive = btnState;
        }


        private void SendSinglePress() => SendKeyboardCommands(_ButtonJoyAction.SinglePressKeyBindings);
        private void SendDoublePress() => SendKeyboardCommands(_ButtonJoyAction.DoublePressKeyBindings);
        private void SendLongPress() => SendKeyboardCommands(_ButtonJoyAction.LongPressKeyBindings);
    }
}
