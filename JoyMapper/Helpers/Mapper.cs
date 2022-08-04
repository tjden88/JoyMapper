using System;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;
using JoyMapper.ViewModels.JoyActions;
using SharpDX.DirectInput;

namespace JoyMapper.Helpers
{
    internal static class Mapper
    {
        public static JoyActionBase ToModel(this JoyActionViewModelBase vm) =>
            vm switch
            {
                AxisJoyActionViewModel axis => new AxisJoyAction()
                {
                    Axis = axis.Axis,
                    StartValue = axis.StartValue,
                    EndValue = axis.EndValue,
                    OnRangeKeyBindings = axis.OnRangeKeys.KeyBindings.ToList(),
                    OutOfRangeKeyBindings = axis.OutOfRangeKeys.KeyBindings.ToList(),
                },
                SimpleButtonJoyActionViewModel sb => new SimpleButtonJoyAction()
                {
                    Button = sb.Button,
                    PressKeyBindings = sb.PressKeys.KeyBindings.ToList(),
                    ReleaseKeyBindings = sb.ReleaseKeys.KeyBindings.ToList(),
                },
                ExtendedButtonJoyActionViewModel extb => new ExtendedButtonJoyAction()
                {
                    Button = extb.Button,
                    SinglePressKeyBindings = extb.SinglePressKeys.KeyBindings.ToList(),
                    DoublePressKeyBindings = extb.DoublePressKeys.KeyBindings.ToList(),
                    LongPressKeyBindings = extb.LongPressKeys.KeyBindings.ToList(),
                },
                _ => throw new NotSupportedException("Неизвестный тип данных")
            };

        public static JoyActionViewModelBase ToViewModel(this JoyActionBase model)
        {
            if (model is AxisJoyAction axis)
            {
                return new AxisJoyActionViewModel
                {
                    Axis = axis.Axis, 
                    StartValue = axis.StartValue,
                    EndValue = axis.EndValue,
                    OnRangeKeys =
                    {
                        KeyBindings = new(axis.OnRangeKeyBindings)
                    },
                    OutOfRangeKeys =
                    {
                        KeyBindings = new(axis.OutOfRangeKeyBindings),
                    }
                };
            }
            throw new ArgumentOutOfRangeException(nameof(model));
        }


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
}
