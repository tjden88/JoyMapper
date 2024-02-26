namespace JoyMapper.Models;

/// <summary>
/// Изменение в состоянии джойстика
/// </summary>
public record JoyStateData(string JoyName, int Id, int Value)
{

    /// <summary>
    /// Buttons: 1-128
    /// Pov1: 200
    /// Pov2: 201
    /// Axes: 300-307
    /// </summary>
    public int Id { get; } = Id;

    /// <summary>
    /// Buttons: 0 - unpressed, other - pressed
    /// </summary>
    public int Value { get; } = Value;


    public bool? IsButtonPressed
    {
        get
        {
            if (Id > 128)
                return null;

            return Value > 0;
        }
    }


    public JoyAxises? Axis => Id switch
    {
        300 => JoyAxises.X,
        301 => JoyAxises.Y,
        302 => JoyAxises.Z,
        303 => JoyAxises.Rx,
        304 => JoyAxises.Ry,
        305 => JoyAxises.Rz,
        306 => JoyAxises.Slider1,
        307 => JoyAxises.Slider2,
        _ => null
    };


    public bool IsPov1Data => Id == 200;

    public bool IsPov2Data => Id == 201;


    public bool IsAxisData => Id >= 300;

    public bool IsButtonData => Id <= 128;
}