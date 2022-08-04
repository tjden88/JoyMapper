using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.JoyActions
{
    internal abstract class JoyActionViewModelBase : ViewModel
    {
        #region JoyName : string - Имя джойстика

        /// <summary>Имя джойстика</summary>
        private string _JoyName;

        /// <summary>Имя джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => Set(ref _JoyName, value);
        }

        #endregion

        /// <summary>Описание действия</summary>
        public abstract string Description { get; }

        /// <summary> Есть ли назначенные кнопки </summary>
        public abstract bool HasKeyBindings { get; }

    }
}
