using JoyMapper.Models.Base;
using JoyMapper.Models.PatternActions.Base;

namespace JoyMapper.Models.Listeners;

/// <summary>
/// Модель для отслеживания привязок и выполнения связанных действий
/// </summary>
/// <param name="BindingBase"></param>
/// <param name="ActionBase"></param>
public record ActionJoyBinding(PatternActionBase Action, bool IsActive);