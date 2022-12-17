using System.Collections.Generic;
using System.Linq;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.UserControls;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows
{
    public class EditPatternViewModel : WindowViewModel
    {
        public EditPatternViewModel(PatternActionViewModel PatternActionViewModel, DataManager DataManager, JoyBindingViewModel JoyBindingViewModel)
        {
            this.PatternActionViewModel = PatternActionViewModel;
            this.JoyBindingViewModel = JoyBindingViewModel;

            GroupsNames = DataManager.JoyPatterns
                .Select(p => p.GroupName)
                .Distinct()
                .Where(p => p != null);

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

        #endregion

    }
}
