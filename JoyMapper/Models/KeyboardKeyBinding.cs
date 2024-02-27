using System.Windows.Input;

namespace JoyMapper.Models;

public class KeyboardKeyBinding
{
    public enum KeyboardAction
    {
        KeyPress,
        KeyUp,
        MousePress,
        MouseUp,
        MouseScrollUp,
        MouseScrollDown
    }

    public Key KeyCode { get; set; }

    public MouseButton MouseButton { get; set; }

    public KeyboardAction Action { get; set; }

    public int Delay { get; set; }

    public override string ToString()
    {
        return Action switch
        {
            KeyboardAction.KeyPress => $"Нажать - {KeyCode}",
            KeyboardAction.KeyUp => $"Отпустить - {KeyCode}",
            KeyboardAction.MousePress => $"Нажать мышь - {MouseButton}",
            KeyboardAction.MouseUp => $"Отпустить мышь - {MouseButton}",
            KeyboardAction.MouseScrollUp => "Скролл мыши вверх",
            KeyboardAction.MouseScrollDown => "Скролл мыши вниз",
            _ => null,
        };
    }
}