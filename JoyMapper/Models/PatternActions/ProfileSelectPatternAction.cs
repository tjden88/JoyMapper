using System;
using System.Linq;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels;
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


    public override PatternActionViewModelBase ToViewModel()
    {
        var data = App.Services.GetRequiredService<DataManager>();
        return new ProfileSelectPatternActionViewModel
        {
            PressProfile = PressProfileId == PrevProfileId
                ? ProfileSelectPatternActionViewModel.PreviousProfile
                : data.Profiles.FirstOrDefault(p => p.Id == PressProfileId),

            ReleaseProfile = ReleaseProfileId == PrevProfileId
                ? ProfileSelectPatternActionViewModel.PreviousProfile
                : data.Profiles.FirstOrDefault(p => p.Id == ReleaseProfileId),

            AllProfiles = data.Profiles,
        };
    }


    protected override void DoReportMode(bool newBindingState)
    {
        if (newBindingState)
            switch (PressProfileId)
            {
                case PrevProfileId:
                    ReportMessage?.Invoke("Переключение на предыдущий профиль");
                    break;
                case > 0:
                    ReportMessage?.Invoke("Переключение на выбранный профиль");
                    break;
            }
        else
        {
            switch (ReleaseProfileId)
            {
                case PrevProfileId:
                    ReportMessage?.Invoke("Переключение на предыдущий профиль");
                    break;
                case > 0:
                    ReportMessage?.Invoke("Переключение на выбранный профиль");
                    break;
            }
        }
    }

    #region Work

    private Profile _PressProfile;

    private Profile _ReleaseProfile;

    private MainWindowViewModel _MainWindowViewModel;

    protected override void Initialize(IServiceProvider Services)
    {
        _PreviousProfile = null;
        var vm = Services.GetRequiredService<MainWindowViewModel>();
        _MainWindowViewModel = vm;
        if (PressProfileId > 0)
            _PressProfile = _MainWindowViewModel.Profiles.FirstOrDefault(p => p.Id == PressProfileId);

        if (ReleaseProfileId > 0)
            _ReleaseProfile = _MainWindowViewModel.Profiles.FirstOrDefault(p => p.Id == ReleaseProfileId);

    }


    protected override void DoWorkMode(bool newBindingState)
    {
        if (newBindingState)
        {
            if(PressProfileId == PrevProfileId && _PreviousProfile != null)
                _MainWindowViewModel.StartProfileCommand.Execute(_PreviousProfile);
            else if (_PressProfile != null)
            {
                _PreviousProfile = _MainWindowViewModel.ActiveProfile;
                _MainWindowViewModel.StartProfileCommand.Execute(_PressProfile);
            }
        }
        else
        {
            if (ReleaseProfileId == PrevProfileId && _PreviousProfile != null)
                _MainWindowViewModel.StartProfileCommand.Execute(_PreviousProfile);
            else if (_ReleaseProfile != null)
            {
                _PreviousProfile = _MainWindowViewModel.ActiveProfile;
                _MainWindowViewModel.StartProfileCommand.Execute(_ReleaseProfile);
            }
        }
    }


    #endregion
}