using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions.Base;


/// <summary>
/// Базовый класс для возможных действий паттернов
/// </summary>
public abstract class PatternActionBase 
{


    public abstract PatternActionViewModelBase ToViewModel();
}