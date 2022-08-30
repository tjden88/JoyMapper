using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class EditModificatorWindowViewModel : WindowViewModel
    {


        public EditModificatorWindowViewModel() => Title = "Добавление модификатора";

        public EditModificatorWindowViewModel(Modificator modificator)
        {
            Title = $"Редактирование модификатора {modificator.Name}";
            Id = modificator.Id;
            Name = modificator.Name;
            JoyName = modificator.JoyName;
            Button = modificator.Button;
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


        #region Button : JoyButton - Кнопка джойстика

        /// <summary>Кнопка джойстика</summary>
        private JoyButton _Button;

        /// <summary>Кнопка джойстика</summary>
        public JoyButton Button
        {
            get => _Button;
            set => IfSet(ref _Button, value).CallPropertyChanged(nameof(BtnText));
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


        public string BtnText => Button is null
            ? "Не назначено"
            : $"{JoyName} ({Button})";

        #endregion

        #region Commands

        #region AsyncCommand SaveCommand - Сохранить модификатор

        /// <summary>Сохранить модификатор</summary>
        private AsyncCommand _SaveCommand;

        /// <summary>Сохранить модификатор</summary>
        public AsyncCommand SaveCommand => _SaveCommand
            ??= new AsyncCommand(OnSaveCommandExecutedAsync, CanSaveCommandExecute, "Сохранить модификатор");

        /// <summary>Проверка возможности выполнения - Сохранить модификатор</summary>
        private bool CanSaveCommandExecute() => true;

        /// <summary>Логика выполнения - Сохранить модификатор</summary>
        private async Task OnSaveCommandExecutedAsync(CancellationToken cancel)
        {
            if (Button == null || JoyName == null)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Не определена кнопка модификатора");
                return;
            }


            var name = Name?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Введите имя модификатора");
                return;
            }

            Name = name;
            ChangesSaved = true;
        }

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
            var joyButton = new JoyActionAdderService().MapJoyButton(out var joyName);
            if (joyButton == null) return;

            JoyName = joyName;
            Button = joyButton;
        }

        #endregion

        #endregion
    }
}
