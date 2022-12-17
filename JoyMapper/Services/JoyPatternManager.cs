using JoyMapper.ViewModels.Windows;

namespace JoyMapper.Services;

/// <summary>
/// Создание, изменение и удаление паттернов
/// </summary>
public class JoyPatternManager
{
    private readonly AppWindowsService _AppWindowsService;

    public JoyPatternManager(AppWindowsService AppWindowsService)
    {
        _AppWindowsService = AppWindowsService;
    }

    public JoyPattern AddPattern()
    {
        var patternWindow = _AppWindowsService.EditPatternWindow;
        patternWindow.Owner = App.ActiveWindow;

        if(patternWindow.ShowDialog() != true) return null;

        var pattern = BuildPattern(patternWindow.ViewModel);
        return pattern;
    }

    private JoyPattern BuildPattern(EditPatternViewModel ViewModel)
    {
        var pattern = new JoyPattern
        {
            Name = ViewModel.PatternName,
            Binding = ViewModel.JoyBinding,
            PatternAction = ViewModel.PatternActionView.ViewModel.SelectedPatternAction.ToModel(),
        };
        return pattern;
    }
}