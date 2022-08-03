using System.Collections.Generic;
using JoyMapper.Models.Base;

namespace JoyMapper.Models
{
    /// <summary>
    /// Действие джойстика, содержащее команды на нажатие и отпускание
    /// </summary>
    internal class SimpleButtonJoyAction : JoyAction
    {
        public JoyButton Button { get; set; }

        public List<KeyboardKeyBinding> PressKeyBindings { get; set; }

        public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }
    }

    /// <summary>
    /// Действие джойстика, содержащее команды на нажатие, двойное и долгое нажатие клавиши
    /// </summary>
    internal class ExtendedButtonJoyAction : JoyAction
    {
        public JoyButton Button { get; set; }

        public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }
        public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; }
        public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; }
    }

    /// <summary>
    /// Действие джойстика, привязанное к интервалу оси контроллера
    /// </summary>
    internal class AxisJoyAction : JoyAction
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

        public Axises Axis { get; set; }

        public int StartValue { get; set; }

        public int EndValue { get; set; }

    }
}
