using appfotball5;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace appfotboll5
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DataService dataService;

        public MainViewModel(DataService dataService)
        {
            this.dataService = dataService;

            AddMatchCommand = new RelayCommand(AddMatchCommand);
            UpdateMatchCommand = new RelayCommand(UpdateMatchCommand);
            RemoveMatchCommand = new RelayCommand(RemoveMatchCommand);

            LoadData();
        }

        public ObservableCollection<MatchViewModel> Matches { get; set; }

        public MatchViewModel SelectedMatch { get; set; }

        public ICommand AddMatchCommand { get; private set; }
        public ICommand UpdateMatchCommand { get; private set; }
        public ICommand RemoveMatchCommand { get; private set; }

        private void AddMatch()
        {
            try
            {
                dataService.AddMatch(SelectedMatch);
                RefreshMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding match: {ex.Message}");
            }
        }

        private void UpdateMatch()
        {
            try
            {
                dataService.UpdateMatch(SelectedMatch);
                RefreshMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating match: {ex.Message}");
            }
        }

        private void RemoveMatch()
        {
            try
            {
                dataService.RemoveMatch(SelectedMatch.MatchID);
                RefreshMatches();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing match: {ex.Message}");
            }
        }

        private void LoadData()
        {
            Matches = new ObservableCollection<MatchViewModel>(dataService.GetMatches());
            OnPropertyChanged(nameof(Matches));
        }

        private void RefreshMatches()
        {
            LoadData();
        }
    }
}
