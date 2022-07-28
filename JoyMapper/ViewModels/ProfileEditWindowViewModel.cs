using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using JoyMapper.Models;
using JoyMapper.Services;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class ProfileEditWindowViewModel : WindowViewModel
    {
        private readonly PatternService _PatternService = new();

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
                PatternId = p.Id,
                Description = p.JoyName + " - Кнопка " + p.JoyKey
            });

            SelectedPatterns = new(mapped);
            Name = profile.Name;
        }

        #endregion


        #region AsyncCommand SaveProfileCommandCommand - Сохранить профиль

        /// <summary>Сохранить профиль</summary>
        private AsyncCommand _SaveProfileCommandCommand;

        /// <summary>Сохранить профиль</summary>
        public AsyncCommand SaveProfileCommand => _SaveProfileCommandCommand
            ??= new AsyncCommand(OnSaveProfileCommandCommandExecutedAsync, CanSaveProfileCommandCommandExecute, "Сохранить профиль");

        /// <summary>Проверка возможности выполнения - Сохранить профиль</summary>
        private bool CanSaveProfileCommandCommandExecute() => true;

        /// <summary>Логика выполнения - Сохранить профиль</summary>
        private async Task OnSaveProfileCommandCommandExecutedAsync(CancellationToken cancel)
        {
            var wnd = App.ActiveWindow;
            if (string.IsNullOrWhiteSpace(Name))
            {
                await WPRMessageBox.InformationAsync(wnd, "Введите имя профиля");
                return;
            }

            if (!SelectedPatterns.Any(p => p.IsSelected))
            {
                await WPRMessageBox.InformationAsync(wnd, "Не выбрано ни одного паттерна!");
                return;
            }

            Name = Name.Trim();
            wnd.DialogResult = true;
        }

        #endregion


        #region Command AddPatternCommand - Добавить паттерн

        /// <summary>Добавить паттерн</summary>
        private Command _AddPatternCommand;

        /// <summary>Добавить паттерн</summary>
        public Command AddPatternCommand => _AddPatternCommand
            ??= new Command(OnAddPatternCommandExecuted, CanAddPatternCommandExecute, "Добавить паттерн");

        /// <summary>Проверка возможности выполнения - Добавить паттерн</summary>
        private bool CanAddPatternCommandExecute() => true;

        /// <summary>Логика выполнения - Добавить паттерн</summary>
        private void OnAddPatternCommandExecuted()
        {
            var added = _PatternService.AddPattern();
            if (added == null) return;

            SelectedPatterns.Add(new SelectedPatternViewModel
            {
                PatternName = added.Name,
                IsSelected = true,
                PatternId = added.Id
            });
        }

        #endregion

    }
}
