﻿using JoyMapper.Models;
using JoyMapper.ViewModels.Windows;

namespace JoyMapper.Services.Data;

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
        var patternWindow = _AppWindowsService.EditPattern;
        patternWindow.Owner = App.ActiveWindow;

        if (patternWindow.ShowDialog() != true) return null;

        var pattern = BuildPattern(patternWindow.ViewModel);
        //_DataManager.AddJoyPattern(pattern);
        return pattern;
    }

    public JoyPattern EditPattern(JoyPattern Pattern)
    {
        var patternWindow = _AppWindowsService.EditPattern;
        var viewModel = patternWindow.ViewModel;
        viewModel.PatternName = Pattern.Name;
        viewModel.JoyBinding = Pattern.Binding;
        viewModel.Title = $"Редактировние паттерна: {Pattern.Name}";
        viewModel.PatternActionView.ViewModel.SetSelectedPatternAction(Pattern.PatternAction.ToViewModel());
        patternWindow.Owner = App.ActiveWindow;

        if (patternWindow.ShowDialog() != true) return null;

        var pattern = BuildPattern(viewModel);
        //_DataManager.UpdateJoyPattern(pattern);
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