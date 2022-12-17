using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class PatternActionViewModel : ViewModel
{

    public PatternActionViewModel()
    {
        AllPatternActions = new List<PatternActionBase>
        {
            new SimpleKeySenderPatternAction(),
            new ExtendedKeySenderPatternAction(),
        };

        SelectedPatternAction = AllPatternActions.First();
    }


    #region AllPatternActions : ICollection<PatternActionBase> - Все существующие виды действий паттернов

    /// <summary>Все существующие виды действий паттернов</summary>
    private ICollection<PatternActionBase> _AllPatternActions;

    /// <summary>Все существующие виды действий паттернов</summary>
    public ICollection<PatternActionBase> AllPatternActions
    {
        get => _AllPatternActions;
        set => Set(ref _AllPatternActions, value);
    }

    #endregion



    #region SelectedPatternAction : PatternActionBase - Выбранный паттерн

    /// <summary>Выбранный паттерн</summary>
    private PatternActionBase _SelectedPatternAction;

    /// <summary>Выбранный паттерн</summary>
    public PatternActionBase SelectedPatternAction
    {
        get => _SelectedPatternAction;
        set => Set(ref _SelectedPatternAction, value);
    }

    #endregion

    
    
}