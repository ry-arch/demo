using System.Data.SqlClient;

namespace ChitaiGorod
{
    public static class DbHelper
    {
        private static readonly string connectionString =
            @"Server=DESKTOP-UGSNVBG\SQLEXPRESS;
              Database=Chitai;
              Trusted_Connection=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}