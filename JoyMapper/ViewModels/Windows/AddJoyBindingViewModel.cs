using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models.JoyBindings;
using WPR.MVVM.ViewModels;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels.Windows
{
    public class AddJoyBindingViewModel : WindowViewModel
    {
        private readonly IJoystickStateManager _JoystickStateManager;
        private readonly IJoyBindingsWatcher _JoyBindingsWatcher;

        private bool _IsWatching;

        public AddJoyBindingViewModel(IJoystickStateManager JoystickStateManager, IJoyBindingsWatcher JoyBindingsWatcher)
        {
            _JoystickStateManager = JoystickStateManager;
            _JoyBindingsWatcher = JoyBindingsWatcher;
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
            set => IfSet(ref _JoyBinding, value)
                .CallPropertyChanged(nameof(JoyName))
                .CallPropertyChanged(nameof(Description));
        }

        #endregion

        public string JoyName => JoyBinding?.JoyName ?? "Нажмите кнопку или сдвиньте ось джойстика";

        public string Description => JoyBinding?.Description;


        #endregion


        #region Commands

        #region Command AcceptButtonCommand - Принять изменения

        /// <summary>Принять изменения</summary>
        private Command _AcceptButtonCommand;

        /// <summary>Принять изменения</summary>
        public Command AcceptButtonCommand => _AcceptButtonCommand
            ??= new Command(OnAcceptButtonCommandExecuted, CanAcceptButtonCommandExecute, "Принять изменения");

        /// <summary>Проверка возможности выполнения - Принять изменения</summary>
        private bool CanAcceptButtonCommandExecute() => JoyBinding != null;

        /// <summary>Логика выполнения - Принять изменения</summary>
        private void OnAcceptButtonCommandExecuted()
        {
            // TODO
        }


        #endregion


        #endregion


        public void StartWatching()
        {
            var allBindings = new List<JoyBindingBase>();
            var connectedJoys = _JoystickStateManager.GetConnectedJoysticks();

            foreach (var connectedJoy in connectedJoys) 
                allBindings.AddRange(AllJoyBindings(connectedJoy));

            _IsWatching = true;
            _JoyBindingsWatcher.StartWatching(allBindings);
            Task.Run(WatchChanges);

        }

        public void StopWatching() => _IsWatching = false;

        private async Task WatchChanges()
        {
            while (_IsWatching)
            {
                var changes = _JoyBindingsWatcher.GetChanges();
                if (changes.FirstOrDefault() is { IsActive: true } change)
                {
                    Debug.WriteLine(change.Description);
                    JoyBinding = change;
                    Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
                }
                await Task.Delay(100);
            }
        }

        #region AllBindings

        private List<JoyBindingBase> AllJoyBindings(string JoyName)
        {
            var list = new List<JoyBindingBase>();

            // Кнопки
            for (var i = 1; i <= 128; i++)
            {
                list.Add(new ButtonJoyBinding
                {
                    JoyName = JoyName,
                    ButtonNumber = i
                });
            }

            var powValues = new[] { 0, 4500, 9000, 13500, 18000, 22500, 27000, 31500 };

            // POW
            foreach (var powValue in powValues)
            {
                list.Add(new PowJoyBinding
                {
                    JoyName = JoyName,
                    PowNumber = PowJoyBinding.PowNumbers.Pow1,
                    PowValue = powValue
                });

                list.Add(new PowJoyBinding
                {
                    JoyName = JoyName,
                    PowNumber = PowJoyBinding.PowNumbers.Pow2,
                    PowValue = powValue
                });
            }

            // Оси
            foreach (AxisJoyBinding.Axises axis in Enum.GetValues(typeof(AxisJoyBinding.Axises)))
            {
                list.Add(new AxisJoyBinding
                {
                    Axis = axis,
                    StartValue = 0,
                    EndValue = 20000,
                    JoyName = JoyName
                });
                list.Add(new AxisJoyBinding()
                {
                    Axis = axis,
                    StartValue = 45000,
                    EndValue = 65535,
                    JoyName = JoyName
                });
            }

            return list;
        }

        #endregion

    }
}
