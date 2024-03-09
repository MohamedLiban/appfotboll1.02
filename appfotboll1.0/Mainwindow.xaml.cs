using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Input;
using appfotboll5DataAccess;
using System.Windows.Controls;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.X509;
using System.Data.Common;
using appfotboll5Models;
using System.Xml.Linq;

namespace appfotball5
{

    public partial class MainWindow : Window
    {
        public List<string> Options { get; set; }

        private readonly MySqlConnection connection;
        private readonly MySqlDataAdapter adapter;
        private readonly DataSet dataSet;
        private string searchTxt;

        // Add RelayCommand for AddPlayer command
        public ICommand AddPlayerCommand { get; private set; }
        public ICommand SearchCommand { get; private set; }  // Added SearchCommand

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Options = new List<string> { "Option 1", "Option 2", "Option 3" };

            connection = DatabaseHelper.GetConnection();
            adapter = new MySqlDataAdapter();
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
            AddTeams();
        }

        private void AddTeams()
        {
            string query = "SELECT * FROM team";
            adapter.SelectCommand = new MySqlCommand(query, connection);
            adapter.Fill(dataSet, "team");


            Options = GetColumnValues<string>("team", "TeamName");
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
                adapter.SelectCommand = new MySqlCommand(query, connection);
                adapter.Fill(dataSet, "Match");

                matchesDataGrid.ItemsSource = dataSet.Tables["Match"].DefaultView;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading matches data: {ex.Message}");
            }
            finally
            {
                connection.Close();
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
                adapter.SelectCommand = new MySqlCommand(query, connection);
                adapter.Fill(dataSet, "alteredPlayer");

                playersDataGrid.ItemsSource = dataSet.Tables["alteredPlayer"].DefaultView;
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading players data: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            dataSet.Reset();
            LoadData();
        
        }

        private void InsertPlayer_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the TextBox and ComboBox
            string name = playerName.Text;
            string selectedOption = cmbOptions.SelectedItem as string;
            int teamId = 0;

            // Add your logic here to insert the changes into your data storage
            // For now, let's display a message box with the selected values
            MessageBox.Show($"Name: {name}\nSelected Option: {selectedOption}");
            

            var table = dataSet.Tables["team"];

            foreach (DataRow row in table.Rows)
            {
                if ((string) row["TeamName"] == selectedOption)
                {
                    teamId = (int)row["TeamId"];
                }
            }

            // Replace "YourPlayerTableName" with the actual player table name
            var CommandText = "INSERT INTO fotboll.Player (Team_TeamID, PlayerName) VALUES (@TeamId, @PlayerName)";

            var cmd = new MySqlCommand(CommandText, connection);
            cmd.Connection = connection;

            // Add parameters to the command to prevent SQL injection
            cmd.Parameters.AddWithValue("@TeamId", teamId);
            cmd.Parameters.AddWithValue("@PlayerName", name);

            try
            {
                // Execute the INSERT query
                connection.Open();
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
                // Close the connection
                name = null;
                selectedOption = null;
                connection.Close();
            }
        }

        private void InsertTeam_Click(object sender, RoutedEventArgs e)
        {
            // Get the values from the TextBox and ComboBox
            string name = TeamName.Text;

            var CommandText = "INSERT INTO fotboll.team (TeamName) VALUES (@TeamName)";

            var cmd = new MySqlCommand(CommandText, connection);
            cmd.Connection = connection;

            // Add parameters to the command to prevent SQL injection
            cmd.Parameters.AddWithValue("@TeamName", name);

            try
            {
                // Execute the INSERT query
                connection.Open();
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
                // Close the connection
                name = null;
                connection.Close();
            }
        }

        public List<T> GetColumnValues<T>(string tableName, string columnName)
        {
            List<T> columnValues = new List<T>();

            // Check if the specified table exists in the DataSet
            if (dataSet.Tables.Contains(tableName))
            {
                DataTable table = dataSet.Tables[tableName];

                // Check if the specified column exists in the table
                if (table.Columns.Contains(columnName))
                {
                    foreach (DataRow row in table.Rows)
                    {

                        T columnvalue = (T)row[columnName]; 
                        // Add the value of the specified column for each row
                        columnValues.Add(columnvalue);
                    }
                }
                else
                {
                    Console.WriteLine($"Column '{columnName}' not found in the table '{tableName}'.");
                }
            }
            else
            {
                Console.WriteLine($"Table '{tableName}' not found in the DataSet.");
            }

            return columnValues;
        }
    }
}