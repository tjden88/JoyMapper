using JoyMapper.Models;
using JoyMapper.ViewModels.Windows;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение и удаление паттернов
/// </summary>
public class JoyPatternManager
{
    private readonly AppWindowsService _AppWindowsService;
    private readonly DataManager _DataManager;

    public JoyPatternManager(AppWindowsService AppWindowsService, DataManager DataManager)
    {
        _AppWindowsService = AppWindowsService;
        _DataManager = DataManager;
    }

    public JoyPattern AddPattern()
    {
        var patternWindow = _AppWindowsService.EditPatternWindow;
        patternWindow.Owner = App.ActiveWindow;

        if (patternWindow.ShowDialog() != true) return null;

        var pattern = BuildPattern(patternWindow.ViewModel);
        _DataManager.AddJoyPattern(pattern);
        return pattern;
    }

    public JoyPattern EditPattern(JoyPattern Pattern)
    {
        var patternWindow = _AppWindowsService.EditPatternWindow;
        var viewModel = patternWindow.ViewModel;
        viewModel.PatternName = Pattern.Name;
        viewModel.JoyBinding = Pattern.Binding;
        viewModel.Title = $"Редактировние паттерна: {Pattern.Name}";
        viewModel.PatternActionView.ViewModel.SetSelectedPatternAction(Pattern.PatternAction.ToViewModel());
        patternWindow.Owner = App.ActiveWindow;

        if (patternWindow.ShowDialog() != true) return null;

        var pattern = BuildPattern(viewModel);
        _DataManager.UpdateJoyPattern(pattern);
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