using JoyMapper.Helpers;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.ViewModels.JoyActions;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;
using static JoyMapper.ViewModels.EditPatternWindowViewModel;

namespace JoyMapper.ViewModels.Windows
{
    public class EditPatternViewModel : WindowViewModel
    {
        private readonly AppWindowsService _AppWindowsService;

        public EditPatternViewModel(AppWindowsService AppWindowsService)
        {
            _AppWindowsService = AppWindowsService;
        }

        #region Prop

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


        #region JoyBinding : JoyBindingBase - Действие кнопки или оси

        /// <summary>Действие кнопки или оси</summary>
        private JoyBindingBase _JoyBinding;

        /// <summary>Действие кнопки или оси</summary>
        public JoyBindingBase JoyBinding
        {
            get => _JoyBinding;
            set => IfSet(ref _JoyBinding, value)
                .CallPropertyChanged(nameof(JoyBindingText))
            ;
        }

        #endregion


        #region JoyBindingText - string

        public string JoyBindingText => JoyBinding is null ? "Не назначено" : $"{JoyBinding.JoyName} - {JoyBinding.Description}";

        #endregion


        #region ActionIsActive : bool - Назначенное действие джойстика сейчас активно

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        private bool _ActionIsActive;

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        public bool ActionIsActive
        {
            get => _ActionIsActive;
            set => IfSet(ref _ActionIsActive, value)
                .CallPropertyChanged(nameof(ActionIsActiveText));
        }

        /// <summary> Текст активности назначенного действия </summary>
        public string ActionIsActiveText => ActionIsActive
            ? "Активно"
            : "Неактивно";


        #endregion

        #endregion


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

            var wnd = _AppWindowsService.AddJoyBinding;

            wnd.Owner = _AppWindowsService.ActiveWindow;

            if (!wnd.ShowDialog() == true)
                return;

            JoyBinding = wnd.ViewModel.JoyBinding;

        }

        #endregion

    }
}
