using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using JoyMapper.Models;
using JoyMapper.Services;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class MainWindowViewModel : ViewModel
    {

        public MainWindowViewModel()
        {
        }

        private readonly PatternService _PatternService = new();


        #region Props

        #region Profiles : ObservableCollection<Profile> - Список профилей

        /// <summary>Список профилей</summary>
        private ObservableCollection<Profile> _Profiles;

        /// <summary>Список профилей</summary>
        public ObservableCollection<Profile> Profiles
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


        #region KeyPatterns : ObservableCollection<KeyPattern> - Список паттернов

        /// <summary>Список паттернов</summary>
        private ObservableCollection<KeyPattern> _KeyPatterns;

        /// <summary>Список паттернов</summary>
        public ObservableCollection<KeyPattern> KeyPatterns
        {
            get => _KeyPatterns;
            set => Set(ref _KeyPatterns, value);
        }

        #endregion


        #region SelectedPattern : KeyPattern - Выбранный паттерн

        /// <summary>Выбранный паттерн</summary>
        private KeyPattern _SelectedPattern;

        /// <summary>Выбранный паттерн</summary>
        public KeyPattern SelectedPattern
        {
            get => _SelectedPattern;
            set => Set(ref _SelectedPattern, value);
        }

        #endregion



        #endregion


        #region Commands

        #region Command LoadDataCommand - Загрузить данные

        /// <summary>Загрузить данные</summary>
        private Command _LoadProfilesCommand;

        /// <summary>Загрузить данные</summary>
        public Command LoadDataCommand => _LoadProfilesCommand
            ??= new Command(OnLoadProfilesCommandExecuted, CanLoadProfilesCommandExecute, "Загрузить данные");

        /// <summary>Проверка возможности выполнения - Загрузить данные</summary>
        private bool CanLoadProfilesCommandExecute() => true;

        /// <summary>Логика выполнения - Загрузить данные</summary>
        private void OnLoadProfilesCommandExecuted()
        {
            Profiles = new(App.DataManager.Profiles);
            KeyPatterns = new(App.DataManager.KeyPatterns);
        }

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
            var vm = new ProfileEditWindowViewModel();
            var wnd = new ProfileEditWindow()
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };
            if (wnd.ShowDialog() != true) return;

            var profile = new Profile
            {
                Name = vm.Name,
                KeyPatternsIds = vm.SelectedPatterns
                    .Where(p => p.IsSelected)
                    .Select(p => p.PatternId)
                    .ToList()
            };

            App.DataManager.AddProfile(profile);
            LoadDataCommand.Execute();
            SelectedProfile = Profiles.Last();

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


        #region Command CreatePatternCommand - Создать паттерн

        /// <summary>Создать паттерн</summary>
        private Command _CreatePatternCommand;

        /// <summary>Создать паттерн</summary>
        public Command CreatePatternCommand => _CreatePatternCommand
            ??= new Command(OnCreatePatternCommandExecuted, CanCreatePatternCommandExecute, "Создать паттерн");

        /// <summary>Проверка возможности выполнения - Создать паттерн</summary>
        private bool CanCreatePatternCommandExecute() => true;

        /// <summary>Логика выполнения - Создать паттерн</summary>
        private void OnCreatePatternCommandExecuted()
        {
            var added = _PatternService.AddPattern();
            if (added == null) return;
            KeyPatterns.Add(added);
            SelectedPattern = added;
        }

        #endregion


        #region Command EditPatternCommand - Редактировать паттерн

        /// <summary>Редактировать паттерн</summary>
        private Command _EditPatternCommand;

        /// <summary>Редактировать паттерн</summary>
        public Command EditPatternCommand => _EditPatternCommand
            ??= new Command(OnEditPatternCommandExecuted, CanEditPatternCommandExecute, "Редактировать паттерн");

        /// <summary>Проверка возможности выполнения - Редактировать паттерн</summary>
        private bool CanEditPatternCommandExecute() => SelectedPattern != null;

        /// <summary>Логика выполнения - Редактировать паттерн</summary>
        private void OnEditPatternCommandExecuted()
        {
            var wnd = new AddPattern()
            {
                Owner = Application.Current.MainWindow,
                Title = $"Редактировать паттерн {SelectedPattern.Name}",
                JoyButton = SelectedPattern.JoyKey,
                JoyName = SelectedPattern.JoyName,
                PatternName = SelectedPattern.Name,
                PressKeyBindings = new(SelectedPattern.PressKeyBindings),
                ReleaseKeyBindings = new(SelectedPattern.ReleaseKeyBindings),

            };
            if (wnd.ShowDialog() != true) return;

            var pattern = new KeyPattern
            {
                JoyKey = wnd.JoyButton,
                JoyName = wnd.JoyName,
                PressKeyBindings = wnd.PressKeyBindings.ToList(),
                ReleaseKeyBindings = wnd.ReleaseKeyBindings.ToList(),
                Name = wnd.PatternName,
                Id = SelectedPattern.Id
            };
            App.DataManager.UpdateKeyPattern(pattern);

            LoadDataCommand.Execute();
            SelectedPattern = KeyPatterns.Last();

        }

        #endregion


        #region Command CopyPatternCommand - Создать копию паттерна

        /// <summary>Создать копию паттерна</summary>
        private Command _CopyPatternCommand;

        /// <summary>Создать копию паттерна</summary>
        public Command CopyPatternCommand => _CopyPatternCommand
            ??= new Command(OnCopyPatternCommandExecuted, CanCopyPatternCommandExecute, "Создать копию паттерна");

        /// <summary>Проверка возможности выполнения - Создать копию паттерна</summary>
        private bool CanCopyPatternCommandExecute() => SelectedPattern != null;

        /// <summary>Логика выполнения - Создать копию паттерна</summary>
        private void OnCopyPatternCommandExecuted()
        {
            App.DataManager.AddKeyPattern(SelectedPattern);
            LoadDataCommand.Execute();
            SelectedPattern = KeyPatterns.Last();
        }

        #endregion


        #region Command DeletePatternCommand - Удалить паттерн

        /// <summary>Удалить паттерн</summary>
        private Command _DeletePatternCommand;

        /// <summary>Удалить паттерн</summary>
        public Command DeletePatternCommand => _DeletePatternCommand
            ??= new Command(OnDeletePatternCommandExecuted, CanDeletePatternCommandExecute, "Удалить паттерн");

        /// <summary>Проверка возможности выполнения - Удалить паттерн</summary>
        private bool CanDeletePatternCommandExecute() => SelectedPattern != null;

        /// <summary>Логика выполнения - Удалить паттерн</summary>
        private void OnDeletePatternCommandExecuted()
        {
            if (MessageBox.Show($"Удалить паттерн {SelectedPattern.Name}?",
                    "Подтвердите удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.DataManager.RemoveKeyPattern(SelectedPattern.Id);
                LoadDataCommand.Execute();
                SelectedPattern = null;

            }
        }

        #endregion

        #endregion

    }
}
