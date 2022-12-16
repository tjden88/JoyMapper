using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models.JoyBindings;
using WPR.MVVM.ViewModels;

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
            set => Set(ref _JoyBinding, value);
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

        public void StopWatching()
        {
            _IsWatching = false;
        }

        private async Task WatchChanges()
        {
            while (_IsWatching)
            {
                var timer = Stopwatch.StartNew();
                var changes = _JoyBindingsWatcher.GetChanges();
                if (changes.FirstOrDefault() is { } change)
                {
                    Debug.WriteLine(change.Description);
                    JoyBinding = change;
                }
                Debug.WriteLine(timer.ElapsedMilliseconds);
                await Task.Delay(100);
            }
        }

        #region AllActions

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
