using System.Linq;
using JoyMapper.Models;
using JoyMapper.ViewModels;
using JoyMapper.Views;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение, удаление профилей
/// </summary>
public class ProfilesManager
{
    private readonly DataSerializer _DataSerializer;
    private readonly AppWindowsService _AppWindowsService;

    public ProfilesManager(DataSerializer DataSerializer, AppWindowsService AppWindowsService)
    {
        _DataSerializer = DataSerializer;
        _AppWindowsService = AppWindowsService;
    }

    public Profile AddProfile()
    {
        var vm = new ProfileEditWindowViewModel();
        var wnd = new ProfileEditWindow
        {
            Owner = _AppWindowsService.ActiveWindow,
            DataContext = vm
        };
        if (wnd.ShowDialog() != true) return null;

        var profile = new Profile
        {
            Name = vm.Name,
            PatternsIds = vm.SelectedPatterns
                .Where(p => p.IsSelected)
                .Select(p => p.PatternId)
                .ToList()
        };
        return profile;
    }

    public Profile CopyProfile(Profile profile)
    {
        var newProf = _DataSerializer.CopyObject(profile);
        return newProf;
    }

    public Profile UpdateProfile(int Id)
    {
        var vm = new ProfileEditWindowViewModel(Id);
        var wnd = new ProfileEditWindow
        {
            Owner = _AppWindowsService.ActiveWindow,
            DataContext = vm
        };
        if (wnd.ShowDialog() != true) return null;

        var profile = new Profile
        {
            Name = vm.Name,
            PatternsIds = vm.SelectedPatterns
                .Where(p => p.IsSelected)
                .Select(p => p.PatternId)
                .ToList(),
            Id = Id,
        };
        return profile;
    }

}