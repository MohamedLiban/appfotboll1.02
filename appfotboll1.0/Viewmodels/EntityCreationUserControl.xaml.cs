using appfotboll5DataAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace appfotball5
{
    public partial class EntityCreationUserControl : UserControl
{
    private readonly MySqlConnection connection;

    public EntityCreationUserControl()
    {
        InitializeComponent();
        connection = DatabaseHelper.GetConnection();
    }


    private void CreateEntity_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Determine the selected entity type
            string entityType = ((ComboBoxItem)entityTypeComboBox.SelectedItem)?.Content.ToString();

            // Insert the new entity into the respective database table
            string query = "";

            switch (entityType)
            {
                case "Team":
                    query = "INSERT INTO fotboll.team (TeamName) VALUES (@TeamName);";
                    break;
                // Add cases for Player and Match based on your requirements

                default:
                    throw new ArgumentException("Invalid entity type selected.");
            }

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@TeamName", entitySpecificTextBox.Text);
                // Add parameters for Player and Match based on your requirements

                connection.Open();
                command.ExecuteNonQuery();
            }

            MessageBox.Show($"New {entityType} added successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding new entity: {ex.Message}");
        }
        finally
        {
            connection.Close();
        }
    }
}
}
