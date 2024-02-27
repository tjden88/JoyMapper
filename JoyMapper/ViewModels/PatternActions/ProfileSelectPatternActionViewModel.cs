using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Переключение профилей во время выполнения
/// </summary>
public class ProfileSelectPatternActionViewModel : PatternActionViewModelBase
{
    public override string Name => "Переключить профиль";
    public override string Description => "Переключение на другой профиль при активации или деактивации действия";

    private static readonly Profile _PreviousProfile = new()
    {
        Id = -1,
        Name = "Предыдущий профиль",
    };


    public ProfileSelectPatternActionViewModel(ProfileSelectPatternAction model = null)
    {
        var data = App.Services.GetRequiredService<DataManager>();
        AllProfiles = data.Profiles;

        if (model == null)
            return;

        PressProfile = model.PressProfileId == -1
            ? _PreviousProfile
            : data.Profiles.FirstOrDefault(p => p.Id == model.PressProfileId);

        ReleaseProfile = model.ReleaseProfileId == -1
            ? _PreviousProfile
            : data.Profiles.FirstOrDefault(p => p.Id == model.ReleaseProfileId);

    }


    #region PressProfile : Profile - Профиль на нажатие

    /// <summary>Профиль на нажатие</summary>
    private Profile _PressProfile;

    /// <summary>Профиль на нажатие</summary>
    public Profile PressProfile
    {
        get => _PressProfile;
        set => IfSet(ref _PressProfile, value)
            .CallPropertyChanged(nameof(PressProfileIsCustom))
        ;
    }

    public bool PressProfileIsNull
    {
        get => PressProfile == null;
        set
        {
            if (value) PressProfile = null;
        }
    }

    public bool PressProfileIsPrevious
    {
        get => PressProfile == _PreviousProfile;
        set
        {
            if (value) PressProfile = _PreviousProfile;
        }
    }

    public bool PressProfileIsCustom
    {
        get => AllProfiles.Any() && PressProfile is { Id: > 0 };

        set
        {
            if (value)
                PressProfile = AllProfiles.FirstOrDefault();
        }
    }

    #endregion


    #region ReleaseProfile : Profile - Профиль на отпускание

    /// <summary>Профиль на отпускание</summary>
    private Profile _ReleaseProfile;

    /// <summary>Профиль на отпускание</summary>
    public Profile ReleaseProfile
    {
        get => _ReleaseProfile;
        set => IfSet(ref _ReleaseProfile, value)
            .CallPropertyChanged(nameof(ReleaseProfileIsCustom));
    }

    public bool ReleaseProfileIsNull
    {
        get => ReleaseProfile == null;
        set
        {
            if (value) ReleaseProfile = null;
        }
    }

    public bool ReleaseProfileIsPrevious
    {
        get => ReleaseProfile == _PreviousProfile;
        set
        {
            if (value) ReleaseProfile = _PreviousProfile;
        }
    }

    public bool ReleaseProfileIsCustom
    {
        get => AllProfiles.Any() && ReleaseProfile is { Id: > 0 };

        set
        {
            if (value)
                ReleaseProfile = AllProfiles.FirstOrDefault();
        }
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