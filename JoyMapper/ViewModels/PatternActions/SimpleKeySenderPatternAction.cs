using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternActionViewModelBase : PatternActionViewModelBase
{
    public override string Name => "Простой триггер";

    public override string Description => "Отправка команд клавиатуры при активации и деактивации действия";

    public ActionKeysBindingViewModel PressKeyBindings { get; set; } = new("Команды при активации");

    public ActionKeysBindingViewModel ReleaseKeyBindings { get; set; } = new("Команды при деактивации");


    public override PatternActionBase ToModel()
    {
        throw new System.NotImplementedException();
    }
}