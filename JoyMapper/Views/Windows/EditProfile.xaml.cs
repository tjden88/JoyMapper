using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
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
        LoadData(DataManager.JoyPatterns.ToList());
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void LoadData(ICollection<JoyPattern> Patterns)
    {

        var mapped = Patterns.Select(p => new EditProfileWindowViewModel.SelectedPatternViewModel
        {
            PatternName = p.Name,
            PatternId = p.Id,
            Description = p.Binding.ToString(),
            GroupName = p.GroupName,
        });
        ViewModel.SelectedPatterns = new(mapped);

        var groups = Patterns
            .Where(p=>!string.IsNullOrEmpty(p.GroupName))
            .GroupBy(p => p.GroupName);

        ViewModel.SelectedPatternsGroups = new(groups.Select(g =>
            new EditProfileWindowViewModel.SelectedPatternGroupViewModel()
            {
                Name = g.Key,
                PatternsCount = g.Count(),
            }));
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
            PatternGroups = ViewModel.SelectedPatternsGroups
                .Where(pg=>pg.IsSelected)
                .Select(pg=>pg.Name)
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

        foreach (var selectedGroup in ViewModel.SelectedPatternsGroups)
            selectedGroup.IsSelected = model.PatternGroups.Contains(selectedGroup.Name);

        ((CollectionView)OtherPatternsList.ItemsSource).Refresh();
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

    private void ViewSource_OnFilter(object sender, FilterEventArgs e)
    {
        if (e.Item is not EditProfileWindowViewModel.SelectedPatternViewModel ptrn)
        {
            e.Accepted = false;
            return;
        }

        if (string.IsNullOrEmpty(ptrn.GroupName))
        {
            e.Accepted = true;
            return;
        }

        var selectedGroups = ViewModel.SelectedPatternsGroups
            .Where(g => g.IsSelected)
            .Select(g => g.Name);
        e.Accepted = !selectedGroups.Any(g=>g.Equals(ptrn.GroupName));
    }

    private void GroupSelect_OnChecked(object sender, RoutedEventArgs e)
    {
        ((CollectionView)OtherPatternsList.ItemsSource).Refresh();
    }
}