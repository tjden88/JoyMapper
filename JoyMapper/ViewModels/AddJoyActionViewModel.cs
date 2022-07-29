using JoyMapper.Models;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddJoyActionViewModel : ViewModel
    {

        #region Prop

        #region JoyName : string - Имя джойстика

        /// <summary>Имя джойстика</summary>
        private string _JoyName = "Джойстик...";

        /// <summary>Имя джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => Set(ref _JoyName, value);
        }

        #endregion


        #region JoyAction : JoyAction - Назначаемое действие джойстика

        /// <summary>Назначаемое действие джойстика</summary>
        private JoyAction _JoyAction;

        /// <summary>Назначаемое действие джойстика</summary>
        public JoyAction JoyAction
        {
            get => _JoyAction;
            set => IfSet(ref _JoyAction, value)
                .CallPropertyChanged(nameof(JoyActionName));
        }

        #endregion


        /// <summary>Имя действия джойстика</summary>
        public string JoyActionName => JoyAction?.ActionText() ?? "Действие...";


        #region Accepted : bool - Принять привязку

        /// <summary>Принять привязку</summary>
        private bool _Accepted;

        /// <summary>Принять привязку</summary>
        public bool Accepted
        {
            get => _Accepted;
            private set => Set(ref _Accepted, value);
        }

        #endregion


        #endregion


        #region Command AcceptButtonCommand - Принять изменения

        /// <summary>Принять изменения</summary>
        private Command _AcceptButtonCommand;

        /// <summary>Принять изменения</summary>
        public Command AcceptButtonCommand => _AcceptButtonCommand
            ??= new Command(OnAcceptButtonCommandExecuted, CanAcceptButtonCommandExecute, "Принять изменения");

        /// <summary>Проверка возможности выполнения - Принять изменения</summary>
        private bool CanAcceptButtonCommandExecute() => JoyName != null && JoyAction != null;

        /// <summary>Логика выполнения - Принять изменения</summary>
        private void OnAcceptButtonCommandExecuted()
        {
            Accepted = true;
        }

        #endregion

    }
}
