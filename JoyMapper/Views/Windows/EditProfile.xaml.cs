﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.Windows;
using WPR.Dialogs;

namespace JoyMapper.Views.Windows;

/// <summary>
/// Логика взаимодействия для EditProfile.xaml
/// </summary>
public partial class EditProfile : IEditModel<Profile>
{
    public EditProfileWindowViewModel ViewModel { get; }

    public EditProfile(EditProfileWindowViewModel viewModel, DataManager DataManager)
    {
        ViewModel = viewModel;
        LoadData(DataManager.JoyPatterns);
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void LoadData(IEnumerable<JoyPattern> Patterns)
    {

        var mapped = Patterns.Select(p => new EditProfileWindowViewModel.SelectedPatternViewModel
        {
            PatternName = p.Name,
            PatternId = p.Id,
            Description = p.Binding.ToString(),
        });
        ViewModel.SelectedPatterns = new(mapped);
    }


    public Profile GetModel()
    {
        var profile = new Profile
        {
            Id = ViewModel.Id,
            Name = ViewModel.Name,
            Description = ViewModel.Description,
            PatternsIds = ViewModel.SelectedPatterns
                .Where(sp=>sp.IsSelected)
                .Select(sp=>sp.PatternId)
                .ToList(),
        };
        return profile;
    }

    public void SetModel(Profile model)
    {
        ViewModel.Id = model.Id;
        ViewModel.Name = model.Name;
        ViewModel.Description = model.Description;

        foreach (var selectedPattern in ViewModel.SelectedPatterns) 
            selectedPattern.IsSelected = model.PatternsIds.Contains(selectedPattern.PatternId);

        ViewModel.Title = $"Редактирование профиля {model.Name}";
    }

    private async void ButtonSave_OnClick(object Sender, RoutedEventArgs E)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.Name))
        {
            await WPRDialogHelper.InformationAsync(this, "Введите имя профиля");
            return;
        }

        if (!ViewModel.SelectedPatterns.Any(p => p.IsSelected))
        {
            await WPRDialogHelper.InformationAsync(this, "Не выбрано ни одного паттерна!");
            return;
        }

        ViewModel.Name = ViewModel.Name.Trim();
        DialogResult = true;
    }
}