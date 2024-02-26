using System.Collections.Generic;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.PatternActions.Base;

namespace JoyMapper.Models.Listeners;

/// <summary>
/// Модель для прослушивателя привязок
/// </summary>
public record ListenerJoyBinding(JoyBindingBase BindingBase, PatternActionBase ActionBase, int ModificatorId, ICollection<int> ForbiddenExecuteModificatorsIds);