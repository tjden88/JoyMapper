using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.Listeners;

/// <summary>
/// Модель привязки с модификатором - для прослушивателя привязок
/// </summary>
public record ModificatedJoyBinding(JoyBindingBase BindingBase, int? ModificatorId = null);