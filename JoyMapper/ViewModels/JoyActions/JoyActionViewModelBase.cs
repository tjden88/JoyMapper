using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.JoyActions
{
    internal abstract class JoyActionViewModelBase : ViewModel
    {

        /// <summary>Описание действия</summary>
        public abstract string Description { get; }

        /// <summary> Есть ли назначенные кнопки </summary>
        public abstract bool HasKeyBindings { get; }

    }
}
