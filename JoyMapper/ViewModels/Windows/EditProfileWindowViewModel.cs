using System.Collections.ObjectModel;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows;

public class EditProfileWindowViewModel : WindowViewModel
{
    private readonly AppWindowsService _AppWindowsService;
    private readonly DataManager _DataManager;

    public EditProfileWindowViewModel(AppWindowsService AppWindowsService, DataManager DataManager)
    {
        _AppWindowsService = AppWindowsService;
        _DataManager = DataManager;
        Title = "Добавление профиля";
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
        var joyPattern = _DataManager.AddJoyPattern();

        if(joyPattern == null) return;

        SelectedPatterns.Add(new SelectedPatternViewModel
        {
            IsSelected = true,
            Description = joyPattern.Binding.ToString(),
            PatternId = joyPattern.Id,
            PatternName = joyPattern.Name,
        });

        _AppWindowsService.GetViewModel<MainWindowViewModel>().JoyPatterns.Add(joyPattern);
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