using System.Collections.Generic;
using JoyMapper.Models;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Переключение профилей во время выполнения
/// </summary>
public class ProfileSelectPatternActionViewModel : PatternActionViewModelBase
{
    public override string Name => "Переключить профиль";
    public override string Description => "Переключение на другой профиль при активации или деактивации действия";

    public static Profile PreviousProfile = new()
    {
        Id = -1,
        Name = "Предыдущий профиль",
    };


    #region PressProfile : Profile - Профиль на нажатие

    /// <summary>Профиль на нажатие</summary>
    private Profile _PressProfile;

    /// <summary>Профиль на нажатие</summary>
    public Profile PressProfile
    {
        get => _PressProfile;
        set => Set(ref _PressProfile, value);
    }

    #endregion


    #region ReleaseProfile : Profile - Профиль на отпускание

    /// <summary>Профиль на отпускание</summary>
    private Profile _ReleaseProfile;

    /// <summary>Профиль на отпускание</summary>
    public Profile ReleaseProfile
    {
        get => _ReleaseProfile;
        set => Set(ref _ReleaseProfile, value);
    }

    #endregion


    #region AllProfiles : IEnumerable<Profile> - Все профили

    /// <summary>Все профили</summary>
    private IEnumerable<Profile> _AllProfiles;

    /// <summary>Все профили</summary>
    public IEnumerable<Profile> AllProfiles
    {
        get => _AllProfiles;
        set => Set(ref _AllProfiles, value);
    }

    #endregion



    public override PatternActionBase ToModel() =>
        new ProfileSelectPatternAction
        {
            PressProfileId = PressProfile?.Id ?? 0,
            ReleaseProfileId = ReleaseProfile?.Id ?? 0,
        };

    public override bool IsValid(out string ErrorMessage)
    {
        ErrorMessage = "Не выбран профиль для переключения";
        return PressProfile != null || ReleaseProfile != null;
    }
}