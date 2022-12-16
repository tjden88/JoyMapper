using JoyMapper.Models.JoyBindings.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class AddJoyBindingViewModel : WindowViewModel
    {

        public AddJoyBindingViewModel()
        {
            Title = "Назначить кнопку/ось";
        }

        #region Prop

        #region JoyBinding : JoyBindingBase - Привязка кнопки

        /// <summary>Привязка кнопки</summary>
        private JoyBindingBase _JoyBinding;

        /// <summary>Привязка кнопки</summary>
        public JoyBindingBase JoyBinding
        {
            get => _JoyBinding;
            set => Set(ref _JoyBinding, value);
        }

        #endregion

        

        #endregion
    }
}
