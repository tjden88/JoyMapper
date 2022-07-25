using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    public class SelectedPatternViewModel : ViewModel
    {

        #region PatternId : int - ИД паттерна

        /// <summary>ИД паттерна</summary>
        private int _PatternId;

        /// <summary>ИД паттерна</summary>
        public int PatternId
        {
            get => _PatternId;
            set => Set(ref _PatternId, value);
        }

        #endregion

        #region PatternName : string - Имя паттерна

        /// <summary>Имя паттерна</summary>
        private string _PatternName;

        /// <summary>Имя паттерна</summary>
        public string PatternName
        {
            get => _PatternName;
            set => Set(ref _PatternName, value);
        }

        #endregion

        #region IsSelected : bool - Выбран ли паттерн

        /// <summary>Выбран ли паттерн</summary>
        private bool _IsSelected;

        /// <summary>Выбран ли паттерн</summary>
        public bool IsSelected
        {
            get => _IsSelected;
            set => Set(ref _IsSelected, value);
        }

        #endregion
        
    }
}
