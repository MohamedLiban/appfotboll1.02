using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Input;
using appfotboll5DataAccess;
using System.Windows.Controls;

namespace appfotball5
{

    public partial class MainWindow : Window
    {
        private readonly MySqlConnection connection;
        private readonly MySqlDataAdapter matchesDataAdapter, playersDataAdapter, teamsDataAdapter;
        private readonly DataSet dataSet;
        private string searchTxt;

        // Add RelayCommand for AddPlayer command
        public ICommand AddPlayerCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }  // Added SearchCommand

        public MainWindow()
        {
            InitializeComponent();

            connection = DatabaseHelper.GetConnection();
            matchesDataAdapter = new MySqlDataAdapter();
            playersDataAdapter = new MySqlDataAdapter();
            teamsDataAdapter = new MySqlDataAdapter();
            dataSet = new DataSet();

           

            // Initialize Search command
            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
                Application.Current.Shutdown();
            }
        }

        private void LoadData()
        {
            LoadMatchesData();
            LoadPlayersData();
            LoadTeamsData();
        }

        private void LoadMatchesData()
        {
            try
            {

                string query = "SELECT distinct B1.TeamName, B2.TeamName, A.ScoreTeam1, A.ScoreTeam2 FROM fotboll.match A " +
                        "JOIN fotboll.team B1 ON A.Team_TeamID1 = B1.TeamID " +
                        "JOIN fotboll.team B2 ON A.Team_TeamID2 = B2.TeamID;";

                if (!string.IsNullOrEmpty(txtSearch.Text)) 
                {
                    var s = txtSearch.Text.Trim();
                    query = "SELECT distinct B1.TeamName, B2.TeamName, A.ScoreTeam1, A.ScoreTeam2 FROM fotboll.match A " +
                        "JOIN fotboll.team B1 ON A.Team_TeamID1 = B1.TeamID " +
                        "JOIN fotboll.team B2 ON A.Team_TeamID2 = B2.TeamID " +
                        "JOIN fotboll.player C1 ON B1.TeamID = C1.Team_TeamID " +
                        "JOIN fotboll.player C2 ON B2.TeamID = C2.Team_TeamID " +
                        $"WHERE (B1.TeamName LIKE '%{s}%' OR B2.TeamName LIKE '%{s}%') " +
                        $"OR C1.PlayerName LIKE '%{s}%' " +
                        $"OR C2.PlayerName LIKE '%{s}%';";
                }
                matchesDataAdapter.SelectCommand = new MySqlCommand(query, connection);
                matchesDataAdapter.Fill(dataSet, "Match");

                matchesDataGrid.ItemsSource = dataSet.Tables["Match"].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading matches data: {ex.Message}");
            }
        }

        private void LoadPlayersData()
        {
            try
            {
                string query = $"SELECT DISTINCT T.TeamName, P.PlayerName " +
                    $"FROM fotboll.team T " +
                    $"LEFT JOIN fotboll.player P ON T.TeamID = P.Team_TeamID;";

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var s = txtSearch.Text.Trim();
                    query = $"SELECT DISTINCT T.TeamName, P.PlayerName " +
                    $"FROM fotboll.team T " +
                    $"LEFT JOIN fotboll.player P ON T.TeamID = P.Team_TeamID " +
                    $"WHERE T.TeamName LIKE '%{s}%' OR P.PlayerName LIKE '%{s}%';";
                }
                playersDataAdapter.SelectCommand = new MySqlCommand(query, connection);
                playersDataAdapter.Fill(dataSet, "Player");

                playersDataGrid.ItemsSource = dataSet.Tables["Player"].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading players data: {ex.Message}");
            }
        }

        private void LoadTeamsData()
        {
            try
            {
                string query = "SELECT * FROM Team";

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var s = txtSearch.Text.Trim();
                    query = $"SELECT DISTINCT T.* " +
                    $"FROM fotboll.team T " +
                    $"LEFT JOIN fotboll.player P ON T.TeamID = P.Team_TeamID " +
                    $"WHERE T.TeamName LIKE '%{s}%' OR P.PlayerName LIKE '%{s}%';";
                }
                teamsDataAdapter.SelectCommand = new MySqlCommand(query, connection);
                teamsDataAdapter.Fill(dataSet, "Team");

                teamsDataGrid.ItemsSource = dataSet.Tables["Team"].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams data: {ex.Message}");
            }
        }


        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            dataSet.Reset();
            LoadData();
        
        }
    }
}