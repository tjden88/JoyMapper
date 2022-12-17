using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class PatternActionViewModel : ViewModel
{

    public PatternActionViewModel()
    {
        AllPatternActions = new List<PatternActionViewModelBase>
        {
            new SimpleKeySenderPatternActionViewModel(),
        };

        SelectedPatternAction = AllPatternActions.First();
    }


    #region AllPatternActions : ICollection<PatternActionViewModelBase> - Все существующие виды действий паттернов

    /// <summary>Все существующие виды действий паттернов</summary>
    private ICollection<PatternActionViewModelBase> _AllPatternActions;

    /// <summary>Все существующие виды действий паттернов</summary>
    public ICollection<PatternActionViewModelBase> AllPatternActions
    {
        get => _AllPatternActions;
        set => Set(ref _AllPatternActions, value);
    }

    #endregion



    #region SelectedPatternAction : PatternActionViewModelBase - Выбранный паттерн

    /// <summary>Выбранный паттерн</summary>
    private PatternActionViewModelBase _SelectedPatternAction;

    /// <summary>Выбранный паттерн</summary>
    public PatternActionViewModelBase SelectedPatternAction
    {
        get => _SelectedPatternAction;
        set => Set(ref _SelectedPatternAction, value);
    }

    #endregion

    
    
}