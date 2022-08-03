using System.Collections.Generic;

namespace JoyMapper.Models.JoyActions
{
    /// <summary>
    /// Действие джойстика, содержащее команды на нажатие и отпускание
    /// </summary>
    internal class SimpleButtonJoyAction : JoyActionBase
    {
        public JoyButton Button { get; set; }

        public List<KeyboardKeyBinding> PressKeyBindings { get; set; }

        public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

        public override string Description => Button.ToString();

    }
}
