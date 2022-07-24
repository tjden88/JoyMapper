using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using SharpDX.DirectInput;
using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class MainWindowViewModel : ViewModel
    {

        public MainWindowViewModel()
        {
            LoadJoyDevices();
        }

        #region JoyDevices : List<JoyDevice> - Список подключённых контроллеров

        /// <summary>Список подключённых контроллеров</summary>
        private List<JoyDevice> _JoyDevices;

        /// <summary>Список подключённых контроллеров</summary>
        public List<JoyDevice> JoyDevices
        {
            get => _JoyDevices;
            set => Set(ref _JoyDevices, value);
        }

        #endregion



        private void LoadJoyDevices()
        {
            var devices = new DirectInput().GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            JoyDevices = devices
                .Select(d => new JoyDevice
                {
                    Name = d.InstanceName
                })
                .ToList();
        }

    }
}
