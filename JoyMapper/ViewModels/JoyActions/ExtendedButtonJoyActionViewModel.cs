using System.Linq;
using System.Windows.Input;
using JoyMapper.Models;

namespace JoyMapper.ViewModels.JoyActions;

internal class ExtendedButtonJoyActionViewModel : JoyActionViewModelBase
{

    #region Button : JoyButton - Кнопка или указатель вида джойстика

    /// <summary>Кнопка или указатель вида джойстика</summary>
    private JoyButton _Button;

    /// <summary>Кнопка или указатель вида джойстика</summary>
    public JoyButton Button
    {
        get => _Button;
        set => Set(ref _Button, value);
    }

    #endregion


    #region SinglePressKeys : ActionKeysBindingViewModel - Команды при нажатии кнопки

    /// <summary>Команды при нажатии кнопки</summary>
    public ActionKeysBindingViewModel SinglePressKeys { get; } = new("Команды при однократном нажатии кнопки");

    #endregion

    #region DoublePressKeys : ActionKeysBindingViewModel - Команды двойного нажатия кнопки

    /// <summary>Команды двойного нажатия кнопки</summary>
    public ActionKeysBindingViewModel DoublePressKeys { get; } = new("Команды двойного нажатия кнопки");

    #endregion

    #region LongPressKeys : ActionKeysBindingViewModel - Команды долгого нажатия кнопки

    /// <summary>Команды долгого нажатия кнопки</summary>
    public ActionKeysBindingViewModel LongPressKeys { get; } = new("Команды при долгом нажатии кнопки");

    #endregion


    public override string Description => Button.ToString();

    public override bool HasKeyBindings => SinglePressKeys.KeyBindings.Any() || DoublePressKeys.KeyBindings.Any() || LongPressKeys.KeyBindings.Any();

    public override bool IsRecording => SinglePressKeys.IsRecorded || DoublePressKeys.IsRecorded || LongPressKeys.IsRecorded;

    public override void AddKeyBinding(Key key, bool isPress)
    {
        if (SinglePressKeys.IsRecorded)
            SinglePressKeys.KeyBindings.Add(new KeyboardKeyBinding { IsPress = isPress, KeyCode = key });
        if (DoublePressKeys.IsRecorded)
            DoublePressKeys.KeyBindings.Add(new KeyboardKeyBinding { IsPress = isPress, KeyCode = key });
        if (LongPressKeys.IsRecorded)
            LongPressKeys.KeyBindings.Add(new KeyboardKeyBinding { IsPress = isPress, KeyCode = key });
    }
}