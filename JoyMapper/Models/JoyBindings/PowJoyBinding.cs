using System;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг указателя вида
/// </summary>
internal class PowJoyBinding : JoyBindingBase
{
    public enum PowNumbers
    {
        Pow1,
        Pow2,
    }

    /// <summary>
    /// Номер указателя вида
    /// </summary>
    public PowNumbers PowNumber { get; set; }


    /// <summary>
    /// Значение указателя
    /// </summary>
    public int PowValue { get; set; }


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