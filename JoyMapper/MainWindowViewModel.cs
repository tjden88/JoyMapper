using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using JoyMapper.Models;
using JoyMapper.Services;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class MainWindowViewModel : ViewModel
    {

        public MainWindowViewModel()
        {
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


        #region SelectedProfile : Profile - Выбранный профиль

        /// <summary>Выбранный профиль</summary>
        private Profile _SelectedProfile;

        /// <summary>Выбранный профиль</summary>
        public Profile SelectedProfile
        {
            get => _SelectedProfile;
            set => Set(ref _SelectedProfile, value);
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
        private void OnLoadProfilesCommandExecuted() => Profiles = App.DataManager.Profiles;

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


        #region Command CreateProfileCommand - Создать профиль

        /// <summary>Создать профиль</summary>
        private Command _CreateProfileCommand;

        /// <summary>Создать профиль</summary>
        public Command CreateProfileCommand => _CreateProfileCommand
            ??= new Command(OnCreateProfileCommandExecuted, CanCreateProfileCommandExecute, "Создать профиль");

        /// <summary>Проверка возможности выполнения - Создать профиль</summary>
        private bool CanCreateProfileCommandExecute() => true;

        /// <summary>Логика выполнения - Создать профиль</summary>
        private void OnCreateProfileCommandExecuted()
        {
            
        }

        #endregion


        #region Command DeleteProfileCommand - Удалить профиль

        /// <summary>Удалить профиль</summary>
        private Command _DeleteProfileCommand;

        /// <summary>Удалить профиль</summary>
        public Command DeleteProfileCommand => _DeleteProfileCommand
            ??= new Command(OnDeleteProfileCommandExecuted, CanDeleteProfileCommandExecute, "Удалить профиль");

        /// <summary>Проверка возможности выполнения - Удалить профиль</summary>
        private bool CanDeleteProfileCommandExecute() => true;

        /// <summary>Логика выполнения - Удалить профиль</summary>
        private void OnDeleteProfileCommandExecuted()
        {
            
        }

        #endregion

        #endregion

    }
}
