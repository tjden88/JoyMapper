using System;
using System.Linq;
using System.Windows.Input;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.ViewModels.JoyActions
{
    internal class AxisJoyActionViewModel : JoyActionViewModelBase
    {

        #region Axises : AxisJoyAction.Axises - Ось

        /// <summary>Ось</summary>
        private AxisJoyAction.Axises _Axis;

        /// <summary>Ось</summary>
        public AxisJoyAction.Axises Axis
        {
            get => _Axis;
            set => Set(ref _Axis, value);
        }

        #endregion


        #region StartValue : int - Начальное значение

        /// <summary>Начальное значение</summary>
        private int _StartValue = 32768;

        /// <summary>Начальное значение</summary>
        public int StartValue
        {
            get => _StartValue;
            set
            {
                if (Equals(_StartValue, value)) return;
                _StartValue = Math.Min(value, EndValue);

                OnPropertyChanged(nameof(StartValue));
            }
        }

        #endregion


        #region EndValue : int - Конечное значение

        /// <summary>Конечное значение</summary>
        private int _EndValue = 65535;

        /// <summary>Конечное значение</summary>
        public int EndValue
        {
            get => _EndValue;
            set
            {
                if (Equals(_EndValue, value)) return;
                _EndValue = Math.Max(value, StartValue);

                OnPropertyChanged(nameof(EndValue));
            }
        }

        #endregion


        #region OnRangeKeys : ActionKeysBindingViewModel - Команды при входе в назначенный диапазон

        /// <summary>Команды при входе в назначенный диапазон</summary>
        public ActionKeysBindingViewModel OnRangeKeys { get; } = new("Команды при входе в диапазон");

        #endregion


        #region OutOfRangeKeys : ActionKeysBindingViewModel - Команды при выходе из диапазона

        /// <summary>Команды при выходе из диапазона</summary>
        public ActionKeysBindingViewModel OutOfRangeKeys { get; } = new("Команды при выходе из диапазона");

        #endregion


        #region CurrentAxisValue : int - Текущее положение оси

        /// <summary>Текущее положение оси</summary>
        private int _CurrentAxisValue;

        /// <summary>Текущее положение оси</summary>
        public int CurrentAxisValue
        {
            get => _CurrentAxisValue;
            set => Set(ref _CurrentAxisValue, value);
        }

        #endregion


        public override string Description => "Ось " + Axis;
        public override bool HasKeyBindings => OnRangeKeys.KeyBindings.Any() || OutOfRangeKeys.KeyBindings.Any();
        public override bool IsRecording => OnRangeKeys.IsRecorded || OutOfRangeKeys.IsRecorded;
        public override void AddKeyBinding(Key key, bool isPress)
        {
            if (OnRangeKeys.IsRecorded)
                OnRangeKeys.KeyBindings.Add(new KeyboardKeyBinding { Action = isPress ? KeyboardKeyBinding.KeyboardAction.KeyPress : KeyboardKeyBinding.KeyboardAction.KeyUp, KeyCode = key });
            if (OutOfRangeKeys.IsRecorded)
                OutOfRangeKeys.KeyBindings.Add(new KeyboardKeyBinding { Action = isPress ? KeyboardKeyBinding.KeyboardAction.KeyPress : KeyboardKeyBinding.KeyboardAction.KeyUp, KeyCode = key });
        }
    }
}
