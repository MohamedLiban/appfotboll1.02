using MySql.Data.MySqlClient;

namespace appfotboll5DataAccess{
    public class DatabaseHelper
{
    private const string ConnectionString = "Server=localhost;Database=fotboll;Uid=root;Pwd=root;";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }
}
}
