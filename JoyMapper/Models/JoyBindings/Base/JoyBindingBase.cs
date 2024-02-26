using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Models.JoyBindings.Base;

/// <summary>
/// Выбранная кнопка или ось джойстика
/// </summary>
public abstract class JoyBindingBase : ViewModel, IEquatable<JoyBindingBase>
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

    private bool _IsPressed;

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
    [JsonIgnore]
    public bool IsActive
    {
        get => _IsActive;
        protected set => IfSet(ref _IsActive, value)
            .CallPropertyChanged(nameof(IsActiveText));
    }

    #endregion


    [JsonIgnore]
    public string IsActiveText => IsActive ? "Активно" : "Неактивно";


    #region ActivationType : ActivationTypes - Тип активации действия

    /// <summary>Тип активации действия</summary>
    private ActivationTypes _ActivationType;

    /// <summary>Тип активации действия</summary>
    public ActivationTypes ActivationType
    {
        get => _ActivationType;
        set => IfSet(ref _ActivationType, value)
            .Then(() => SetIsActive(_IsPressed));
    }

    #endregion


    /// <summary>
    /// Обновить статус действия с учётом типа активации
    /// Если статус ИЗМЕНИЛСЯ - возвращает true
    /// </summary>
    /// <param name="joyState">Статус джойстика</param>
    public bool SetNewActiveStatus( [NotNull] JoyStateData joyState)
    {
        if (!Equals(joyState.JoyName, JoyName) || !EqualsBindingState(joyState))
            return false;

        var oldStatus = IsActive;
        _IsPressed = IsPressed(joyState);
        SetIsActive(_IsPressed);

        return IsActive != oldStatus;
    }

    protected void SetIsActive(bool isPressed)
    {
        var result = ActivationType switch
        {
            ActivationTypes.Normal => isPressed,
            ActivationTypes.Reverse => !isPressed,
            ActivationTypes.Switch => CheckSwitchStatus(isPressed),
            _ => throw new ArgumentOutOfRangeException(nameof(ActivationType))
        };

        IsActive = result;
    }

    #region Abstract

    /// <summary> Нажата ли кнопка или ось в назначенном диапазоне </summary>
    protected abstract bool IsPressed(JoyStateData joyState);


    /// <summary> Проверить, отностится ли статус джойстика к этой привязке</summary>
    protected abstract bool EqualsBindingState(JoyStateData joyState);


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

    public abstract bool Equals(JoyBindingBase other);

    public override string ToString() => $"{JoyName} ({Description})";
}