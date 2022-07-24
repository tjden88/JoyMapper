using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class ProfileEditWindowViewModel : WindowViewModel
    {
        public ProfileEditWindowViewModel()
        {
            Title = "Редактирование профиля";
        }

        #region Name : string - Имя профиля

        /// <summary>Имя профиля</summary>
        private string _Name;

        /// <summary>Имя профиля</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

        
    }
}
