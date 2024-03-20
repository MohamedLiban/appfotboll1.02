using System;
using System.Collections.Generic;
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
        public List<string> Options { get; set; }

        private readonly MySqlConnection connection;
        private readonly MySqlDataAdapter adapter;
        private readonly DataSet dataSet;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Options = new List<string> { "Ac Milan", "Juventus", "Real Madrid" };

            connection = DatabaseHelper.GetConnection();
            adapter = new MySqlDataAdapter();
            dataSet = new DataSet();

            InitializeDatabase();
            LoadData();
        }

        private void InitializeDatabase()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to the database: {ex.Message}");
                Application.Current.Shutdown();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void LoadData()
        {
            LoadMatchesData();
            LoadPlayersData();
            AddTeams();
        }

        private void AddTeams()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                string query = "SELECT * FROM team";
                adapter.SelectCommand = new MySqlCommand(query, connection);
                adapter.Fill(dataSet, "team");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading teams data: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void LoadMatchesData()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

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
                adapter.SelectCommand = new MySqlCommand(query, connection);
                adapter.Fill(dataSet, "Match");

                matchesDataGrid.ItemsSource = dataSet.Tables["Match"].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading matches data: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void LoadPlayersData()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                string query = $"SELECT DISTINCT T.TeamName, P.PlayerName, P.PlayerID " +
                    $"FROM fotboll.team T " +
                    $"LEFT JOIN fotboll.player P ON T.TeamID = P.Team_TeamID;";

                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    var s = txtSearch.Text.Trim();
                    query = $"SELECT DISTINCT T.TeamName, P.PlayerName, P.PlayerID " +
                    $"FROM fotboll.team T " +
                    $"LEFT JOIN fotboll.player P ON T.TeamID = P.Team_TeamID " +
                    $"WHERE T.TeamName LIKE '%{s}%' OR P.PlayerName LIKE '%{s}%';";
                }
                adapter.SelectCommand = new MySqlCommand(query, connection);
                adapter.Fill(dataSet, "alteredPlayer");

                playersDataGrid.ItemsSource = dataSet.Tables["alteredPlayer"].DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading players data: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            dataSet.Reset();
            LoadData();
        }

        private void InsertPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                string name = playerName.Text;
                string selectedOption = cmbOptions.SelectedItem as string;
                int teamId = 0;

                MessageBox.Show($"Name: {name}\nSelected Option: {selectedOption}");

                var table = dataSet.Tables["team"];

                foreach (DataRow row in table.Rows)
                {
                    if ((string)row["TeamName"] == selectedOption)
                    {
                        teamId = (int)row["TeamId"];
                        break;
                    }
                }

                var CommandText = "INSERT INTO fotboll.Player (Team_TeamID, PlayerName) VALUES (@TeamId, @PlayerName)";

                var cmd = new MySqlCommand(CommandText, connection);
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@TeamId", teamId);
                cmd.Parameters.AddWithValue("@PlayerName", name);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Player inserted successfully.");
                dataSet.Reset();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting player: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void InsertTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                string name = TeamName.Text;

                var CommandText = "INSERT INTO fotboll.team (TeamName) VALUES (@TeamName)";

                var cmd = new MySqlCommand(CommandText, connection);
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@TeamName", name);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Team inserted successfully.");
                dataSet.Reset();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting team: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                DataRowView selectedPlayer = (DataRowView)playersDataGrid.SelectedItem;

                if (selectedPlayer != null)
                {
                    
                    string playerName = selectedPlayer["PlayerName"].ToString();

                    
                    RemovePlayer(playerName);
                }
                else
                {
                    MessageBox.Show("Please select a player to remove.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing player: {ex.Message}");
            }
        }

        private void RemovePlayer(string playerName)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                
                if (IsPlayerInDatabase(playerName))
                {
                    
                    RemovePlayerFromDatabase(playerName);
                }
                else
                {
                    
                    RemovePlayerFromDataTable(playerName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing player: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private bool IsPlayerInDatabase(string playerName)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM fotboll.Player WHERE PlayerName = @PlayerName";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@PlayerName", playerName);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        int count = Convert.ToInt32(result);
                        return count > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking if player exists: {ex.Message}");
                return false;
            }
        }

        private void RemovePlayerFromDatabase(string playerName)
        {
            try
            {
                string commandText = "DELETE FROM fotboll.Player WHERE PlayerName = @PlayerName";
                using (MySqlCommand cmd = new MySqlCommand(commandText, connection))
                {
                    cmd.Parameters.AddWithValue("@PlayerName", playerName);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Player removed successfully from the database.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing player from the database: {ex.Message}");
            }
        }

        private void RemovePlayerFromDataTable(string playerName)
        {
            try
            {
                
                DataRow[] rows = dataSet.Tables["alteredPlayer"].Select($"PlayerName = '{playerName}'");
                foreach (DataRow row in rows)
                {
                    row.Delete();
                }

                
                MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.DeleteCommand = builder.GetDeleteCommand();
                adapter.Update(dataSet, "alteredPlayer");
                MessageBox.Show("Player removed successfully from the local DataTable and database.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating the database: {ex.Message}");
            }
        }
    }
}
