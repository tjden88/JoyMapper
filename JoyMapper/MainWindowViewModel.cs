using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Services;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class MainWindowViewModel : ViewModel
    {

        private readonly DataManager _DataManager = new();

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


        #region Props

        #region Profiles : List<Profile> - Список профилей

        /// <summary>Список профилей</summary>
        private List<Profile> _Profiles;

        /// <summary>Список профилей</summary>
        public List<Profile> Profiles
        {
            get => _Profiles;
            set => Set(ref _Profiles, value);
        }

        #endregion

        #endregion


        #region Commands
        
        #region Command LoadProfilesCommand - Загрузить профили

        /// <summary>Загрузить профили</summary>
        private Command _LoadProfilesCommand;

        /// <summary>Загрузить профили</summary>
        public Command LoadProfilesCommand => _LoadProfilesCommand
            ??= new Command(OnLoadProfilesCommandExecuted, CanLoadProfilesCommandExecute, "Загрузить профили");

        /// <summary>Проверка возможности выполнения - Загрузить профили</summary>
        private bool CanLoadProfilesCommandExecute() => true;

        /// <summary>Логика выполнения - Загрузить профили</summary>
        private void OnLoadProfilesCommandExecuted() => Profiles = _DataManager.Profiles;

        #endregion


        #region Command StartProfileCommand - Запустить профиль

        /// <summary>Запустить профиль</summary>
        private Command _StartProfileCommand;

        /// <summary>Запустить профиль</summary>
        public Command StartProfileCommand => _StartProfileCommand
            ??= new Command(OnStartProfileCommandExecuted, CanStartProfileCommandExecute, "Запустить профиль");

        /// <summary>Проверка возможности выполнения - Запустить профиль</summary>
        private bool CanStartProfileCommandExecute(object p) => p is Profile;

        /// <summary>Логика выполнения - Запустить профиль</summary>
        private void OnStartProfileCommandExecuted(object p)
        {
            var profile = (Profile)p;
        }

        #endregion

        #endregion

    }
}
