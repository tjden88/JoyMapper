using System;
using Newtonsoft.Json;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Models.JoyBindings.Base
{
    /// <summary>
    /// Выбранная кнопка или ось джойстика
    /// </summary>
    public abstract class JoyBindingBase : ViewModel
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


        #region JoyName : string - Имя привязанного джойстика

        /// <summary>Имя привязанного джойстика</summary>
        private string _JoyName;

        /// <summary>Имя привязанного джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => Set(ref _JoyName, value);
        }

        #endregion


        #region IsActive : bool - Активно ли действие с учётом типа активации

        /// <summary>Активно ли действие с учётом типа активации</summary>
        private bool _IsActive;

        /// <summary>Активно ли действие с учётом типа активации</summary>
        public bool IsActive
        {
            get => _IsActive;
            set => Set(ref _IsActive, value);
        }

        #endregion


        #region ActivationType : ActivationTypes - Тип активации действия

        /// <summary>Тип активации действия</summary>
        private ActivationTypes _ActivationType;

        /// <summary>Тип активации действия</summary>
        public ActivationTypes ActivationType
        {
            get => _ActivationType;
            set => Set(ref _ActivationType, value);
        }

        #endregion


        /// <summary>
        /// Обновить статус действия с учётом типа активации
        /// </summary>
        /// <param name="joyState">Статус джойстика</param>
        public bool UpdateIsActive(JoyState joyState)
        {
            var pressed = IsPressed(joyState);

            var result = ActivationType switch
            {
                ActivationTypes.Normal => pressed,
                ActivationTypes.Reverse => !pressed,
                ActivationTypes.Switch => CheckSwitchStatus(pressed),
                _ => throw new ArgumentOutOfRangeException(nameof(ActivationType))
            };

            IsActive = result;
            return result;
        }

        #region Abstract

        /// <summary> Нажата ли кнопка или ось в назначенном диапазоне </summary>
        protected abstract bool IsPressed(JoyState joyState);


        /// <summary>
        /// Описание действия
        /// </summary>
        [JsonIgnore]
        public abstract string Description { get; }

        #endregion

        #region SwitchCheck

        private bool _IsNowPressed; // Для типа активации - переключатель

        /// <summary> Проверка переключателя </summary>
        private bool CheckSwitchStatus(bool pressed)
        {
            switch (pressed)
            {
                case false:
                    _IsNowPressed = false;
                    break;

                case true when !_IsNowPressed && !IsActive:
                   _IsNowPressed = true;
                    return true;
                
                case true when !_IsNowPressed && IsActive:
                    _IsNowPressed = true;
                    return false;
            }
            return IsActive;
        }

        #endregion
    }
}
