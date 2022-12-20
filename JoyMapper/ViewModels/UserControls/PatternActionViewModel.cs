using System.Collections.Generic;
using System.Linq;
using JoyMapper.Interfaces;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class PatternActionViewModel : ViewModel, IEditModel<PatternActionBase>
{

    public PatternActionViewModel()
    {
        AllPatternActions = new List<PatternActionViewModelBase>
        {
            new SimpleKeySenderPatternActionViewModel(),
            new ExtendedKeySenderPatternActionViewModel(),
            new SeqKeySenderViewModel(),
            new ProfileSelectPatternActionViewModel(),

        };

        SelectedPatternAction = AllPatternActions.First();
    }

    #region AllPatternActions : ICollection<PatternActionViewModelBase> - Все существующие виды действий паттернов

    /// <summary>Все существующие виды действий паттернов</summary>
    private List<PatternActionViewModelBase> _AllPatternActions;

    /// <summary>Все существующие виды действий паттернов</summary>
    public List<PatternActionViewModelBase> AllPatternActions
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


    public PatternActionBase GetModel() => 
        SelectedPatternAction.ToModel();


    public void SetModel(PatternActionBase model)
    {
        var viewModel = model.ToViewModel();
        var type = viewModel.GetType();
        var replace = AllPatternActions.First(p => p.GetType() == type);
        var index = AllPatternActions.IndexOf(replace);
        AllPatternActions.Remove(replace);
        AllPatternActions.Insert(index, viewModel);
        SelectedPatternAction = viewModel;
    }

}