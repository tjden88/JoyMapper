using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.PatternActions.Base;
using Newtonsoft.Json;

namespace JoyMapper.Models;

/// <summary> Паттерн привязки джойстика и действий </summary>
public class JoyPattern
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string GroupName { get; set; }

    public JoyBindingBase Binding { get; set; }

    public PatternActionBase PatternAction { get; set; }

    public int? ModificatorId { get; set; }

    public override string ToString() => $"{Binding.JoyName} - {Binding.Description}";

    [JsonIgnore]
    public bool HasModificator => ModificatorId != null ;
}