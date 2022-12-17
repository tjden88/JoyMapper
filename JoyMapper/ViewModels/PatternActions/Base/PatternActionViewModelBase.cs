using JoyMapper.Models.PatternActions.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.PatternActions.Base
{
    /// <summary>
    /// Базовый класс для вьюмоделей действий паттернов
    /// </summary>
    public abstract class PatternActionViewModelBase : ViewModel
    {
        public abstract string Name { get; }

        public abstract string Description { get; }


        public abstract PatternActionBase ToModel();
    }
}
