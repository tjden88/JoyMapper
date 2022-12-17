using JoyMapper.ViewModels.UserControls;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class EditModificatorViewModel : WindowViewModel
    {
        public EditModificatorViewModel(JoyBindingViewModel JoyBindingViewModel)
        {
            this.JoyBindingViewModel = JoyBindingViewModel;
            Title = "Добавление модификатора";
        }

        #region Prop

        public JoyBindingViewModel JoyBindingViewModel { get; }


        #region Id : int - Id редактируемого модификатора

        /// <summary>Id редактируемого модификатора</summary>
        private int _Id;

        /// <summary>Id редактируемого модификатора</summary>
        public int Id
        {
            get => _Id;
            set => Set(ref _Id, value);
        }

        #endregion


        #region Name : string - Имя

        /// <summary>Имя</summary>
        private string _Name;

        /// <summary>Имя</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion


        #endregion
    }
}
