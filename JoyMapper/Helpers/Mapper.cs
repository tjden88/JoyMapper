using JoyMapper.Models;
using SharpDX.DirectInput;

namespace JoyMapper.Helpers;

internal static class Mapper
{

    public static JoyState ToModel(this JoystickState joystickState) =>
        new()
        {
            Buttons = joystickState.Buttons,
            Pow1Value = joystickState.PointOfViewControllers[0],
            Pow2Value = joystickState.PointOfViewControllers[1],
            AxisValues = new JoyState.AxisState()
            {
                X = joystickState.X,
                Y = joystickState.Y,
                Z = joystickState.Z,
                Rx = joystickState.RotationX,
                Ry = joystickState.RotationY,
                Rz = joystickState.RotationZ,
                Slider1 = joystickState.Sliders[0],
                Slider2 = joystickState.Sliders[1],

            }
        };
}