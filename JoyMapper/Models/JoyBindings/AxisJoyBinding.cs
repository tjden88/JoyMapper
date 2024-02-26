using System;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг оси джойстика
/// </summary>
public class AxisJoyBinding : JoyBindingBase
{
    public event EventHandler<int> CurrentValueChanged;

    #region Axis : JoyAxises - Выбранная ось

    /// <summary>Выбранная ось</summary>
    private JoyAxises _Axis;

    /// <summary>Выбранная ось</summary>
    public JoyAxises Axis
    {
        get => _Axis;
        set => IfSet(ref _Axis, value).CallPropertyChanged(nameof(Description));
    }

    #endregion


    #region StartValue : int - Начальное значение активации оси

    /// <summary>Начальное значение активации оси</summary>
    private int _StartValue;

    /// <summary>Начальное значение активации оси</summary>
    public int StartValue
    {
        get => _StartValue;
        set
        {
            if (Equals(_StartValue, value)) return;
            _StartValue = value;
            EndValue = Math.Max(value, EndValue);

            OnPropertyChanged(nameof(StartValue));
        }
    }

    #endregion


    #region EndValue : int - Значение выхода из активированной зоны оси

    /// <summary>Значение выхода из активированной зоны оси</summary>
    private int _EndValue;

    /// <summary>Значение выхода из активированной зоны оси</summary>
    public int EndValue
    {
        get => _EndValue;
        set
        {
            if (Equals(_EndValue, value)) return;
            _EndValue = value;
            StartValue = Math.Min(value, StartValue);

            OnPropertyChanged(nameof(EndValue));
        }
    }

    #endregion


    #region CurrentValue : int - Текущее значение оси

    /// <summary>Текущее значение оси</summary>
    private int _CurrentValue;

    /// <summary>Текущее значение оси</summary>
    public int CurrentValue
    {
        get => _CurrentValue;
        private set => IfSet(ref _CurrentValue, value).Then(v => CurrentValueChanged?.Invoke(this, v));
    }

    #endregion

    

    protected override bool IsPressed(JoyStateData joyState)
    {
        var value = joyState.Value;

        CurrentValue = value;
        return value >= StartValue && value <= EndValue;
    }

    protected override bool EqualsBindingState(JoyStateData joyState) => 
        joyState.Axis is { } axis && axis == Axis;

    public override string Description => $"Ось {Axis}";
    public override bool Equals(JoyBindingBase other)
    {
        if (other is not AxisJoyBinding ab)
            return false;

        return ab.JoyName.Equals(JoyName) && ab.Axis == Axis;
    }
}