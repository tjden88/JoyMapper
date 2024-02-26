using System;
using JoyMapper.Models.Base;

namespace JoyMapper.Models;

/// <summary>
/// Биндинг указателя вида
/// </summary>
public class PowJoyBinding : JoyBindingBase
{
    public enum PowNumbers
    {
        Pow1,
        Pow2,
    }

    #region PowNumber : PowNumbers - Номер указателя вида

    /// <summary>Номер указателя вида</summary>
    private PowNumbers _PowNumber;

    /// <summary>Номер указателя вида</summary>
    public PowNumbers PowNumber
    {
        get => _PowNumber;
        set => IfSet(ref _PowNumber, value).CallPropertyChanged(nameof(Description));
    }

    #endregion


    #region PowValue : int - Значение указателя

    /// <summary>Значение указателя</summary>
    private int _PowValue;

    /// <summary>Значение указателя</summary>
    public int PowValue
    {
        get => _PowValue;
        set => IfSet(ref _PowValue, value).CallPropertyChanged(nameof(Description));
    }

    #endregion


    protected override bool IsPressed(JoyState joyState) => PowNumber switch
    {
        PowNumbers.Pow1 => joyState.Pow1Value == PowValue,
        PowNumbers.Pow2 => joyState.Pow2Value == PowValue,
        _ => throw new ArgumentOutOfRangeException(nameof(PowNumber))
    };


    public override string Description => PowNumber switch
    {
        PowNumbers.Pow1 => "Переключатель вида №1 " + GetPowPoint(PowValue),
        PowNumbers.Pow2 => "Переключатель вида №2 " + GetPowPoint(PowValue),
        _ => throw new ArgumentOutOfRangeException(nameof(PowNumber))
    };

    public override bool Equals(JoyBindingBase other)
    {
        if (other is not PowJoyBinding pb)
            return false;
        return pb.JoyName.Equals(JoyName) && pb.PowNumber == PowNumber && pb.PowValue == PowValue;
    }


    private static string GetPowPoint(int rawValue) =>
        rawValue switch
        {
            0 => "Вверх",
            4500 => "Вверх-Вправо",
            9000 => "Вправо",
            13500 => "Вниз-Вправо",
            18000 => "Вниз",
            22500 => "Вниз-Влево",
            27000 => "Влево",
            31500 => "Вверх-Влево",
            _ => "None"
        };
}