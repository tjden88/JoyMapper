using System;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг оси джойстика
/// </summary>
public class AxisJoyBinding : JoyBindingBase
{
    public enum Axises
    {
        X,
        Y,
        Z,
        Rx,
        Ry,
        Rz,
        Slider1,
        Slider2,
    }

    #region Axis : Axises - Выбранная ось

    /// <summary>Выбранная ось</summary>
    private Axises _Axis;

    /// <summary>Выбранная ось</summary>
    public Axises Axis
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
        private set => Set(ref _CurrentValue, value);
    }

    #endregion

    

    protected override bool IsPressed(JoyState joyState)
    {
        var value = Axis switch
        {
            Axises.X => joyState.AxisValues.X,
            Axises.Y => joyState.AxisValues.Y,
            Axises.Z => joyState.AxisValues.Z,
            Axises.Rx => joyState.AxisValues.Rx,
            Axises.Ry => joyState.AxisValues.Ry,
            Axises.Rz => joyState.AxisValues.Rz,
            Axises.Slider1 => joyState.AxisValues.Slider1,
            Axises.Slider2 => joyState.AxisValues.Slider2,
            _ => throw new ArgumentOutOfRangeException(nameof(Axis))
        };
        CurrentValue = value;
        return value >= StartValue && value <= EndValue;
    }

    public override string Description => $"Ось {Axis}";
}