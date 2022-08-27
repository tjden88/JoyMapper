using System;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers
{
    internal class ExtendedButtonActionWatcher : ActionWatcherBase
    {
        private readonly ExtendedButtonJoyAction _ButtonJoyAction;

        private readonly bool _IsDoublePressActionsExist;

        private readonly int _DoublePressDelay = 400;
        private readonly int _LongPressDelay = 500;

        private Stopwatch _DelayMeter; // Таймер задержки между нажатиями

        private bool _FirstPressHandled; // Первое нажатие поймано

        public override JoyActionBase JoyAction => _ButtonJoyAction;

        public ExtendedButtonActionWatcher(ExtendedButtonJoyAction buttonJoyAction)
        {
            _ButtonJoyAction = buttonJoyAction;
            _IsDoublePressActionsExist = buttonJoyAction.DoublePressKeyBindings?.Any() == true;
        }

        public override void Poll(JoyState joyState, bool SendCommands)
        {
            var isBtnPressed = _ButtonJoyAction.Button.Type switch
            {
                ButtonType.Button => joyState.Buttons[_ButtonJoyAction.Button.Value-1],
                ButtonType.Pow1 => joyState.Pow1Value == _ButtonJoyAction.Button.Value,
                ButtonType.Pow2 => joyState.Pow2Value == _ButtonJoyAction.Button.Value,
                _ => throw new ArgumentOutOfRangeException()
            };

            var prewIsPressed = IsActive;
            IsActive = isBtnPressed;

            // Кнопка не нажата и прошло время двойного клика или
            // Кнопка отпущена после первого нажатия и на двойное нажатие действий не назначено
            if (_FirstPressHandled && !isBtnPressed && (!_IsDoublePressActionsExist || _DelayMeter?.ElapsedMilliseconds > _DoublePressDelay))
            {
                OnActionHandled?.Invoke("Одиночное нажатие");

                Debug.WriteLine("SinglePressSend: " + _ButtonJoyAction.Button);
               if(SendCommands) SendSinglePress();
                _FirstPressHandled = false;
                _DelayMeter = null;
            }
            

            if (!prewIsPressed && isBtnPressed) // Состояние изменилось на нажатое
            {
                if (!_FirstPressHandled) // Регистрируем первое нажатие
                {
                    _FirstPressHandled = true;
                    _DelayMeter = Stopwatch.StartNew();
                }
                else // Второе нажатие
                {
                    OnActionHandled?.Invoke("Двойное нажатие");
                    Debug.WriteLine("DoublePressSend: " + _ButtonJoyAction.Button);
                    if (SendCommands) SendDoublePress();
                    _FirstPressHandled = false;
                    _DelayMeter = null;
                }
            }

            // Кнопка нажата больше времени долгого нажатия
            if (_FirstPressHandled && isBtnPressed && _DelayMeter?.ElapsedMilliseconds > _LongPressDelay)
            {
                OnActionHandled?.Invoke("Долгое нажатие");
                Debug.WriteLine("LongPressSend: " + _ButtonJoyAction.Button);
                if (SendCommands) SendLongPress();
                _FirstPressHandled = false;
                _DelayMeter = null;
            }


        }


        private void SendSinglePress() => SendKeyboardCommands(_ButtonJoyAction.SinglePressKeyBindings);
        private void SendDoublePress() => SendKeyboardCommands(_ButtonJoyAction.DoublePressKeyBindings);
        private void SendLongPress() => SendKeyboardCommands(_ButtonJoyAction.LongPressKeyBindings);
    }
}
