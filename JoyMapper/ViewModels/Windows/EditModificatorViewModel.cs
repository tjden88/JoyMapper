using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class EditModificatorViewModel : WindowViewModel
    {
        private readonly AppWindowsService _AppWindowsService;


        public EditModificatorViewModel(AppWindowsService AppWindowsService)
        {
            _AppWindowsService = AppWindowsService;
            Title = "Добавление модификатора";
        }


        #region Prop

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


        #region JoyBinding : JoyBindingBase - Привязка кнопки к модификатору

        /// <summary>Привязка кнопки к модификатору</summary>
        private JoyBindingBase _JoyBinding;

        /// <summary>Привязка кнопки к модификатору</summary>
        public JoyBindingBase JoyBinding
        {
            get => _JoyBinding;
            set => Set(ref _JoyBinding, value);
        }

        #endregion


        #region ChangesSaved : bool - Изменения приняты

        /// <summary>Изменения приняты</summary>
        private bool _ChangesSaved;

        /// <summary>Изменения приняты</summary>
        public bool ChangesSaved
        {
            get => _ChangesSaved;
            private set => Set(ref _ChangesSaved, value);
        }

        #endregion


        #endregion

        #region Commands


        #region Command AttachJoyButtonCommand - Определить кнопку джойстика

        /// <summary>Определить кнопку джойстика</summary>
        private Command _AttachJoyButtonCommand;

        /// <summary>Определить кнопку джойстика</summary>
        public Command AttachJoyButtonCommand => _AttachJoyButtonCommand
            ??= new Command(OnAttachJoyButtonCommandExecuted, CanAttachJoyButtonCommandExecute, "Определить кнопку джойстика");

        /// <summary>Проверка возможности выполнения - Определить кнопку джойстика</summary>
        private bool CanAttachJoyButtonCommandExecute() => true;

        /// <summary>Логика выполнения - Определить кнопку джойстика</summary>
        private void OnAttachJoyButtonCommandExecuted()
        {
            var bind = _AppWindowsService.GetJoyBinding();
            if (bind is null)
                return;

            JoyBinding = bind;
        }

        #endregion

        #endregion
    }
}
