using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace appfotboll5
{
    public class DataService
    {
        private readonly MySqlConnection connection;

        public DataService(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void AddMatch(MatchViewModel selectedMatch)
        {
            try
            {
                connection.Open();

                string insertQuery = "INSERT INTO `Match` (Result) VALUES (@Result)";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@Result", selectedMatch.Result);

                insertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding match: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void UpdateMatch(MatchViewModel selectedMatch)
        {
            try
            {
                connection.Open();

                string updateQuery = "UPDATE `Match` SET Result = @Result WHERE MatchID = @MatchID";
                MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection);
                updateCommand.Parameters.AddWithValue("@MatchID", selectedMatch.MatchID);
                updateCommand.Parameters.AddWithValue("@Result", selectedMatch.Result);

                updateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating match: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public IEnumerable<MatchViewModel> GetMatches()
        {
            List<MatchViewModel> matches = new List<MatchViewModel>();

            try
            {
                connection.Open();

                string selectQuery = "SELECT MatchID, Result FROM `Match`";
                MySqlCommand selectCommand = new MySqlCommand(selectQuery, connection);

                using (MySqlDataReader reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MatchViewModel match = new MatchViewModel
                        {
                            MatchID = Convert.ToInt32(reader["MatchID"]),
                            Result = reader["Result"].ToString()
                        };

                        matches.Add(match);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting matches: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return matches;
        }

        public void RemoveMatch(int matchID)
        {
            try
            {
                connection.Open();

                string deleteQuery = "DELETE FROM `Match` WHERE MatchID = @MatchID";
                MySqlCommand deleteCommand = new MySqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@MatchID", matchID);

                deleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing match: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
