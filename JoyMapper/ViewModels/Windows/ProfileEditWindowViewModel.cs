using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services.Data;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    internal class ProfileEditWindowViewModel : WindowViewModel
    {
        private static DataManager DataManager => App.DataManager;

        public ProfileEditWindowViewModel()
        {
            Title = "Добавление профиля";
        }

        public ProfileEditWindowViewModel(int id)
        {
            if (id < 1)
                throw new ArgumentOutOfRangeException(nameof(id), "Неверный профиль!");

            Title = "Редактирование профиля";
            Id = id;
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


        #region Description : string - Описание профиля

        /// <summary>Описание профиля</summary>
        private string _Description;

        /// <summary>Описание профиля</summary>
        public string Description
        {
            get => _Description;
            set => Set(ref _Description, value);
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
            var allPatterns = DataManager.JoyPatterns;

            var profile = Id > 0
                ? DataManager.Profiles.First(pr => pr.Id == Id)
                : new Profile();

            var mapped = allPatterns.Select(p => new SelectedPatternViewModel
            {
                IsSelected = profile.PatternsIds.Contains(p.Id),
                PatternName = p.Name,
                PatternId = p.Id,
                Description = p.ToString()
            });

            SelectedPatterns = new(mapped);
            Name = profile.Name;
            Description = profile.Description;
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
            return;
            //var added = _PatternService.AddPattern();
            //if (added == null) return;

            //SelectedPatterns.Add(new SelectedPatternViewModel
            //{
            //    PatternName = added.Name,
            //    IsSelected = true,
            //    PatternId = added.Id,
            //    Description = added.JoyName + " - " + added.JoyAction.Description
            //});
        }

        #endregion


        public class SelectedPatternViewModel : ViewModel
        {

            #region PatternId : int - ИД паттерна

            /// <summary>ИД паттерна</summary>
            private int _PatternId;

            /// <summary>ИД паттерна</summary>
            public int PatternId
            {
                get => _PatternId;
                set => Set(ref _PatternId, value);
            }

            #endregion

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

            #region IsSelected : bool - Выбран ли паттерн

            /// <summary>Выбран ли паттерн</summary>
            private bool _IsSelected;

            /// <summary>Выбран ли паттерн</summary>
            public bool IsSelected
            {
                get => _IsSelected;
                set => Set(ref _IsSelected, value);
            }

            #endregion

            #region Description : string - Описание джойстика и кнопки

            /// <summary>Описание джойстика и кнопки</summary>
            private string _Description;

            /// <summary>Описание джойстика и кнопки</summary>
            public string Description
            {
                get => _Description;
                set => Set(ref _Description, value);
            }

            #endregion


        }
    }
}
