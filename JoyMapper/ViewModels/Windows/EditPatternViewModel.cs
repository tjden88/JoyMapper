using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.Services.Interfaces;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class EditPatternViewModel : WindowViewModel, IDisposable
    {
        private readonly AppWindowsService _AppWindowsService;
        private readonly IJoyBindingsWatcher _BindingsWatcher;

        public EditPatternViewModel(AppWindowsService AppWindowsService, IJoyBindingsWatcher BindingsWatcher)
        {
            _AppWindowsService = AppWindowsService;
            _BindingsWatcher = BindingsWatcher;
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
                .CallPropertyChanged(nameof(IsNormal))
                .CallPropertyChanged(nameof(IsReverse))
                .CallPropertyChanged(nameof(IsSwitch))

                .Then(WatchAction)
            ;
        }

        #endregion


        #region JoyBindingText - string

        public string JoyBindingText => JoyBinding is null ? "Не назначено" : $"{JoyBinding.JoyName} - {JoyBinding.Description}";

        #endregion




        #region ActionType - тип действия

        public bool IsNormal
        {
            get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Normal;
            set
            {
                if (value && JoyBinding != null) JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Normal;
            }
        }

        public bool IsReverse
        {
            get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Reverse;
            set
            {
                if (value && JoyBinding != null) JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Reverse;
            }
        }

        public bool IsSwitch
        {
            get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Switch;
            set
            {
                if (value && JoyBinding != null) JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Switch;
            }
        }

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



        private void WatchAction()
        {
            if (JoyBinding is null)
                return;

            _BindingsWatcher.StopWatching();

            _BindingsWatcher.StartWatching(new[] { JoyBinding });

            Task.Run(CheckStatus);
        }

        private bool _StatusChecked;

        private async Task CheckStatus()
        {
            if (_StatusChecked)
            {
                _StatusChecked = false;
                await Task.Delay(300);
            }
            _StatusChecked = true;
            Debug.WriteLine("Начато отслеживания кнопки");
            while (_StatusChecked)
            {
                _BindingsWatcher.UpdateStatus();
                await Task.Delay(100);
            }
            Debug.WriteLine("Выход из отслеживания кнопки");
        }

        public void Dispose() => _StatusChecked = false;
    }
}
