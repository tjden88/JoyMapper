using System.Windows.Input;

namespace JoyMapper.Models;

public class KeyboardKeyBinding
{
    public enum KeyboardAction
    {
        KeyPress,
        KeyUp,
    }

    public Key KeyCode { get; set; }

    public KeyboardAction Action { get; set; }

    public override string ToString()
    {
        return Action switch
        {
            KeyboardAction.KeyPress => "Нажать - " + KeyCode,
            KeyboardAction.KeyUp => "Отпустить - " + KeyCode,
            _ => null,
        };
    }
}