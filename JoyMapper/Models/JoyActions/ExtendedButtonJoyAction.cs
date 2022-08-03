using System.Collections.Generic;

namespace JoyMapper.Models.JoyActions;

/// <summary>
/// Действие джойстика, содержащее команды на нажатие, двойное и долгое нажатие клавиши
/// </summary>
internal class ExtendedButtonJoyAction : JoyActionBase
{
    public JoyButton Button { get; set; }

    public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }
    public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; }
    public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; }
    public override string Description => Button.ToString();
}