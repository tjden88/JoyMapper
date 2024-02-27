using System;
using System.Linq;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Переключение профилей во время выполнения
/// </summary>
public class ProfileSelectPatternAction : PatternActionBase
{
    private static Profile _PreviousProfile; // Предыдущий профиль для переключения на него

    private const int PrevProfileId = -1; // Обозначение для перехода на предыдущий профиль



    /// <summary> Id профиля при нажатии </summary>
    public int PressProfileId { get; set; }


    /// <summary> Id профиля при отпускании </summary>
    public int ReleaseProfileId { get; set; }


    public override PatternActionViewModelBase ToViewModel() => new ProfileSelectPatternActionViewModel(this);


    protected override void DoReportMode(bool newBindingState)
    {
        ReportMessage?.Invoke(newBindingState ? "Переключение при активации" : "Переключение при декативации");
    }

    #region Work

    private Profile _PressProfile;

    private Profile _ReleaseProfile;

    private IProfileListener _ProfileListener;

    protected override void Initialize(IServiceProvider Services)
    {
        if (LogReportMode) return;
        //_PreviousProfile = null;
        var data = Services.GetRequiredService<DataManager>();
        if (PressProfileId > 0)
            _PressProfile = data.Profiles.FirstOrDefault(p => p.Id == PressProfileId);

        if (ReleaseProfileId > 0)
            _ReleaseProfile = data.Profiles.FirstOrDefault(p => p.Id == ReleaseProfileId);

        _ProfileListener = Services.GetService<IProfileListener>();
    }



    protected override void DoWorkMode(bool newBindingState)
    {
        var newProfileId = newBindingState ? PressProfileId : ReleaseProfileId;
        var prof = newBindingState ? _PressProfile : _ReleaseProfile;

        if (newProfileId == PrevProfileId && _PreviousProfile != null)
            _ProfileListener.StartListenProfile(_PreviousProfile);
        else if (prof != null && _ProfileListener.CurrentProfile != prof)
        {
            _PreviousProfile = _ProfileListener.CurrentProfile;
            _ProfileListener.StartListenProfile(prof);
        }
    }


    #endregion
}