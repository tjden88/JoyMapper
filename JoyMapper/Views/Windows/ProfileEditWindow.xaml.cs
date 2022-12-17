using System.Collections.Generic;
using System.Linq;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.Windows;
using static JoyMapper.ViewModels.Windows.ProfileEditWindowViewModel;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProfileEditWindow.xaml
    /// </summary>
    public partial class ProfileEditWindow : IEditModelWindow<Profile>
    {
        public ProfileEditWindowViewModel ViewModel { get; }

        public ProfileEditWindow(ProfileEditWindowViewModel viewModel, DataManager DataManager)
        {
            ViewModel = viewModel;
            LoadData(DataManager.JoyPatterns);
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void LoadData(IEnumerable<JoyPattern> Patterns)
        {

            var mapped = Patterns.Select(p => new SelectedPatternViewModel
            {
                PatternName = p.Name,
                PatternId = p.Id,
                Description = p.ToString()
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
    }
}
