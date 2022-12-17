using Newtonsoft.Json;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Models.PatternActions.Base;


/// <summary>
/// Базовый класс для возможных действий паттернов
/// </summary>
public abstract class PatternActionBase : ViewModel
{
    [JsonIgnore]
    public abstract string Name { get; }

    [JsonIgnore]
    public abstract string Description { get; }
}