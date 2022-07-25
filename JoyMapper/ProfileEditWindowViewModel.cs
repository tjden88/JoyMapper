using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using JoyMapper.Models;
using JoyMapper.ViewModels;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper
{
    internal class ProfileEditWindowViewModel : WindowViewModel
    {
        public ProfileEditWindowViewModel()
        {
            Title = "Добавление профиля";
        }

        public ProfileEditWindowViewModel(int Id)
        {
            if (Id < 1)
                throw new ArgumentOutOfRangeException(nameof(Id), "Неверный ID профиля!");

            Title = "Редактирование профиля";
            this.Id = Id;
        }

        #region Id : int - ID профиля (0 - новый профиль)

        /// <summary>ID профиля (0 - новый профиль)</summary>
        private int _Id;

        /// <summary>ID профиля (0 - новый профиль)</summary>
        public int Id
        {
            get => _Id;
            set => Set(ref _Id, value);
        }

        #endregion

        
        #region Name : string - Имя профиля

        /// <summary>Имя профиля</summary>
        private string _Name;

        /// <summary>Имя профиля</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion


        #region SelectedPatterns : ObservableCollection<SelectedPatternViewModel> - Список паттернов

        /// <summary>Список паттернов</summary>
        private ObservableCollection<SelectedPatternViewModel> _SelectedPatterns;

        /// <summary>Список паттернов</summary>
        public ObservableCollection<SelectedPatternViewModel> SelectedPatterns
        {
            get => _SelectedPatterns;
            set => Set(ref _SelectedPatterns, value);
        }

        #endregion


        #region Command LoadDataCommand - Загрузить данные профиля

        /// <summary>Загрузить данные профиля</summary>
        private Command _LoadDataCommand;

        /// <summary>Загрузить данные профиля</summary>
        public Command LoadDataCommand => _LoadDataCommand
            ??= new Command(OnLoadDataCommandExecuted, CanLoadDataCommandExecute, "Загрузить данные профиля");

        /// <summary>Проверка возможности выполнения - Загрузить данные профиля</summary>
        private bool CanLoadDataCommandExecute() => true;

        /// <summary>Логика выполнения - Загрузить данные профиля</summary>
        private void OnLoadDataCommandExecuted()
        {
            var allPatterns = App.DataManager.KeyPatterns;

            var profile = Id > 0
                ? App.DataManager.Profiles.First(pr => pr.Id == Id)
                : new Profile();

            var mapped = allPatterns.Select(p => new SelectedPatternViewModel
            {
                IsSelected = profile.KeyPatternsIds.Contains(p.Id),
                PatternName = p.Name,
                PatternId = p.Id
            });

            SelectedPatterns = new(mapped);
            Name = profile.Name;
        }

        #endregion


        #region Command SaveProfileCommand - Сохранить профиль

        /// <summary>Сохранить профиль</summary>
        private Command _SaveProfileCommand;

        /// <summary>Сохранить профиль</summary>
        public Command SaveProfileCommand => _SaveProfileCommand
            ??= new Command(OnSaveProfileCommandExecuted, CanSaveProfileCommandExecute, "Сохранить профиль");

        /// <summary>Проверка возможности выполнения - Сохранить профиль</summary>
        private bool CanSaveProfileCommandExecute() => true;

        /// <summary>Логика выполнения - Сохранить профиль</summary>
        private void OnSaveProfileCommandExecuted()
        {
            var wnd = Application.Current.Windows.Cast<Window>().First(w => w.IsActive);
            if (string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show(wnd, "Введите имя профиля");
                return;
            }

            if (!SelectedPatterns.Any(p => p.IsSelected))
            {
                MessageBox.Show(wnd, "Не выбрано ни одного паттерна!");
                return;
            }

            wnd.DialogResult = true;
        }

        #endregion
        
    }
}
