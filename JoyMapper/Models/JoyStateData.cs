using System;

namespace JoyMapper.Models;

/// <summary>
/// Изменение в состоянии джойстика
/// </summary>
public record JoyStateData(int Id, int Value)
{

    /// <summary>
    /// Buttons: 0-127
    /// Pov1: 128
    /// Pov2: 129
    /// Axes: 130-137
    /// </summary>
    public int Id { get; } = Id;

    /// <summary>
    /// Buttons: 0 - unpressed, 1 - pressed
    /// </summary>
    public int Value { get; } = Value;


    public bool? IsButtonPressed
    {
        get
        {
            if (Id > 127)
                return null;

            return Value == 1;
        }
    }


    public JoyAxises? Axis => Id switch
    {
        130 => JoyAxises.X,
        131 => JoyAxises.Y,
        132 => JoyAxises.Z,
        133 => JoyAxises.Rx,
        134 => JoyAxises.Ry,
        135 => JoyAxises.Rz,
        136 => JoyAxises.Slider1,
        137 => JoyAxises.Slider2,
        _ => null
    };


    public bool IsPov1Data => Id == 128;

    public bool IsPov2Data => Id == 129;


    public bool IsAxisData => Id >= 130;

    public bool IsButtonData => Id <= 127;
}