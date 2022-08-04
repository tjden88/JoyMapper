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
        private int _StartValue;

        /// <summary>Начальное значение</summary>
        public int StartValue
        {
            get => _StartValue;
            set => Set(ref _StartValue, value);
        }

        #endregion


        #region EndValue : int - Конечное значение

        /// <summary>Конечное значение</summary>
        private int _EndValue;

        /// <summary>Конечное значение</summary>
        public int EndValue
        {
            get => _EndValue;
            set => Set(ref _EndValue, value);
        }

        #endregion


        #region OnRangeKeys : ActionKeysBindingViewModel - Команды при входе в назначенный диапазон

        /// <summary>Команды при входе в назначенный диапазон</summary>
        private ActionKeysBindingViewModel _OnRangeKeys = new() {Name = "Команды при входе в диапазон"};

        /// <summary>Команды при входе в назначенный диапазон</summary>
        public ActionKeysBindingViewModel OnRangeKeys
        {
            get => _OnRangeKeys;
            set => Set(ref _OnRangeKeys, value);
        }

        #endregion


        #region OutOfRangeKeys : ActionKeysBindingViewModel - Команды при выходе из диапазона

        /// <summary>Команды при выходе из диапазона</summary>
        private ActionKeysBindingViewModel _OutOfRangeKeys = new() {Name = "Команды при выходе из диапазона"};

        /// <summary>Команды при выходе из диапазона</summary>
        public ActionKeysBindingViewModel OutOfRangeKeys
        {
            get => _OutOfRangeKeys;
            set => Set(ref _OutOfRangeKeys, value);
        }

        #endregion


        #region CurrentAxisValue : int - Текущее положение оси

        /// <summary>Текущее положение оси</summary>
        private int _CurrentAxisValue;

        /// <summary>Текущее положение оси</summary>
        public int CurrentAxisValue
        {
            get => _CurrentAxisValue;
            private set => Set(ref _CurrentAxisValue, value);
        }

        #endregion


        public override string Description => "Ось " + Axis;
    }
}
