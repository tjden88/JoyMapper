using System;

namespace JoyMapper.Models.JoyBindings.Base
{
    /// <summary>
    /// Выбранная кнопка или ось джойстика
    /// </summary>
    public abstract class JoyBindingBase
    {
        /// <summary>
        /// Тип активациии действия - нормальный, обратный (активно при отпускании), переключатель
        /// </summary>
        public enum ActivationTypes
        {
            Normal,
            Reverse,
            Switch
        }


        /// <summary>
        /// Имя привязанного джойстика
        /// </summary>
        public string JoyName { get; set; }


        /// <summary>
        /// Тип активациии действия
        /// </summary>
        public ActivationTypes ActivationType { get; set; }


        /// <summary>
        /// Активно ли действие в текущий момент с учётом типа активации
        /// </summary>
        /// <param name="joyState">Статус джойстика</param>
        /// <returns></returns>
        public bool IsActive(JoyState joyState)
        {
            var pressed = IsPressed(joyState);

            return ActivationType switch
            {
                ActivationTypes.Normal => pressed,
                ActivationTypes.Reverse => !pressed,
                ActivationTypes.Switch => CheckSwitchStatus(pressed),
                _ => throw new ArgumentOutOfRangeException(nameof(ActivationType))
            };
        }

        #region Abstract

        /// <summary> Нажата ли кнопка или ось в назначенном диапазоне </summary>
        protected abstract bool IsPressed(JoyState joyState);


        /// <summary>
        /// Описание действия
        /// </summary>
        public abstract string Description { get; }

        #endregion

        #region SwitchCheck

        private bool _IsNowPressed; // Для типа активации - переключатель

        /// <summary> Проверка переключателя </summary>
        private bool CheckSwitchStatus(bool pressed)
        {
            switch (pressed)
            {
                case true when !_IsNowPressed:
                    _IsNowPressed = true;
                    break;
                case false when _IsNowPressed:
                    _IsNowPressed = false;
                    return true;
            }
            return false;
        }

        #endregion
    }
}
