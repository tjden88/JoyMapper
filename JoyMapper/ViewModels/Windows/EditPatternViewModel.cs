using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.UserControls;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class EditPatternViewModel : WindowViewModel, IDisposable
    {
        private readonly IServiceProvider _Services;

        public EditPatternViewModel(IServiceProvider Services, PatternActionViewModel PatternActionViewModel, DataManager DataManager, JoyBindingViewModel JoyBindingViewModel)
        {
            _Services = Services;
            this.PatternActionViewModel = PatternActionViewModel;
            this.JoyBindingViewModel = JoyBindingViewModel;

            GroupsNames = DataManager.JoyPatterns
                .Select(p => p.GroupName)
                .Distinct()
                .Where(p => p != null);

            Modificators = DataManager.Modificators;

            JoyBindingViewModel.BindingStateChanged += BindingStateChanged;
            PatternActionViewModel.PropertyChanged += PatternActionViewModelOnPropertyChanged;
            SetPatternAction(PatternActionViewModel.SelectedPatternAction.ToModel());

            Title = "Добавить паттерн";
        }


        #region Prop

        public PatternActionViewModel PatternActionViewModel { get; }

        public JoyBindingViewModel JoyBindingViewModel { get; }


        #region Id : int - Идентификатор редактируемого паттерна

        /// <summary>Идентификатор редактируемого паттерна</summary>
        private int _Id;

        /// <summary>Идентификатор редактируемого паттерна</summary>
        public int Id
        {
            get => _Id;
            set => Set(ref _Id, value);
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


        #region GroupsNames : IEnumerable<string> - Все группы

        /// <summary>Все группы</summary>
        private IEnumerable<string> _GroupsNames;

        /// <summary>Все группы</summary>
        public IEnumerable<string> GroupsNames
        {
            get => _GroupsNames;
            set => Set(ref _GroupsNames, value);
        }

        #endregion


        #region GroupName : string - Имя группы паттерна

        /// <summary>Имя группы паттерна</summary>
        private string _GroupName;

        /// <summary>Имя группы паттерна</summary>
        public string GroupName
        {
            get => _GroupName;
            set => Set(ref _GroupName, value);
        }

        #endregion


        #region Modificators : IEnumerable<Modificator> - Модификаторы

        /// <summary>Модификаторы</summary>
        private IEnumerable<Modificator> _Modificators;

        /// <summary>Модификаторы</summary>
        public IEnumerable<Modificator> Modificators
        {
            get => _Modificators;
            set => Set(ref _Modificators, value);
        }

        #endregion


        #region SelectedModificator : Modificator - Выбранный модификатор

        /// <summary>Выбранный модификатор</summary>
        private Modificator _SelectedModificator;

        /// <summary>Выбранный модификатор</summary>
        public Modificator SelectedModificator
        {
            get => _SelectedModificator;
            set => Set(ref _SelectedModificator, value);
        }

        #endregion


        #region WatcherLogText : string - Текст отслеживания событий

        /// <summary>Текст отслеживания событий</summary>
        private string _WatcherLogText;

        /// <summary>Текст отслеживания событий</summary>
        public string WatcherLogText
        {
            get => _WatcherLogText;
            set => Set(ref _WatcherLogText, value);
        }

        #endregion


        #endregion


        #region Command ClearModificatorCommand - Убрать модификатор

        /// <summary>Убрать модификатор</summary>
        private Command _ClearModificatorCommand;

        /// <summary>Убрать модификатор</summary>
        public Command ClearModificatorCommand => _ClearModificatorCommand
            ??= new Command(OnClearModificatorCommandExecuted, CanClearModificatorCommandExecute, "Убрать модификатор");

        /// <summary>Проверка возможности выполнения - Убрать модификатор</summary>
        private bool CanClearModificatorCommandExecute() => SelectedModificator != null;

        /// <summary>Логика выполнения - Убрать модификатор</summary>
        private void OnClearModificatorCommandExecuted() => SelectedModificator = null;

        #endregion


        #region Command ClearLogCommand - Очистить лог действий

        /// <summary>Очистить лог действий</summary>
        private Command _ClearLogCommand;

        /// <summary>Очистить лог действий</summary>
        public Command ClearLogCommand => _ClearLogCommand
            ??= new Command(OnClearLogCommandExecuted, CanClearLogCommandExecute, "Очистить лог действий");

        /// <summary>Проверка возможности выполнения - Очистить лог действий</summary>
        private bool CanClearLogCommandExecute() => true;

        /// <summary>Логика выполнения - Очистить лог действий</summary>
        private void OnClearLogCommandExecuted() => WatcherLogText = null;

        #endregion


        #region ActionWatch


        private PatternActionBase _PatternActionBase;

        private void SetPatternAction(PatternActionBase pattern)
        {
            if (_PatternActionBase != null)
                _PatternActionBase.ReportMessage -= Action_OnReportMessage;

            if (pattern == null)
            {
                _PatternActionBase = null;
                return;
            }

            pattern.Initialize(_Services, true);
            _PatternActionBase = pattern;
            pattern.ReportMessage += Action_OnReportMessage;
        }


        private void Action_OnReportMessage(string message)
        {
            const string mark = ">>> ";
            WatcherLogText = string.Concat(mark, message, Environment.NewLine, _WatcherLogText?.Replace(mark, string.Empty));
        }


        private void BindingStateChanged(bool state) => 
            _PatternActionBase?.BindingStateChanged(state);


        private void PatternActionViewModelOnPropertyChanged(object Sender, PropertyChangedEventArgs E)
        {
            if (E.PropertyName == nameof(PatternActionViewModel.SelectedPatternAction)) 
                SetPatternAction(PatternActionViewModel.SelectedPatternAction.ToModel());
        }

        #endregion


        public void Dispose()
        {
            if (_PatternActionBase != null)
            {
                _PatternActionBase.ReportMessage -= Action_OnReportMessage;
                _PatternActionBase = null;
            }
        }
    }
}
