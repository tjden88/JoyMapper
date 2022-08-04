using System;
using System.Linq;
using JoyMapper.Models.JoyActions;
using JoyMapper.ViewModels.JoyActions;

namespace JoyMapper.Helpers
{
    internal static class Mapper
    {
        public static JoyActionBase ToModelActionBase(this JoyActionViewModelBase vm) =>
            vm switch
            {
                AxisJoyActionViewModel axis => new AxisJoyAction()
                {
                    JoyName = axis.JoyName,
                    Axis = axis.Axis,
                    StartValue = axis.StartValue,
                    EndValue = axis.EndValue,
                    OnRangeKeyBindings = axis.OnRangeKeys.KeyBindings.ToList(),
                    OutOfRangeKeyBindings = axis.OutOfRangeKeys.KeyBindings.ToList(),
                },
                SimpleButtonJoyActionViewModel sb => new SimpleButtonJoyAction()
                {
                    JoyName = sb.JoyName,
                    Button = sb.Button,
                    PressKeyBindings = sb.PressKeys.KeyBindings.ToList(),
                    ReleaseKeyBindings = sb.ReleaseKeys.KeyBindings.ToList(),
                },
                ExtendedButtonJoyActionViewModel extb => new ExtendedButtonJoyAction()
                {
                    JoyName = extb.JoyName,
                    Button = extb.Button,
                    SinglePressKeyBindings = extb.SinglePressKeys.KeyBindings.ToList(),
                    DoublePressKeyBindings = extb.DoublePressKeys.KeyBindings.ToList(),
                    LongPressKeyBindings = extb.LongPressKeys.KeyBindings.ToList(),
                },
                _ => throw new NotSupportedException("Неизвестный тип данных")
            };
    }
}
