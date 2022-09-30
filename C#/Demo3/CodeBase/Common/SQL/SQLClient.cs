using System;
using System.Data;
using System.Data.SqlClient;


namespace Common.SQL
{
    public class SQLClient
    {
        public Exception Error { get; private set; }
        public ConnectionState ConnectionState => sqlConnection.State;


        readonly SqlConnection sqlConnection;


        public SQLClient(string hostName, int port, string catalog, string user, string password)
        {
            string connectionString = string.Format(
               @"
                    Data Source={0},{1};
                    Initial Catalog={2};
                    User id={3};
                    Password={4};
                ", hostName, port, catalog, user, password);

            sqlConnection = new SqlConnection(connectionString);
        }

        public void Connect()
        {
            try
            {
                sqlConnection.Open();
                Error = null;
            }

            catch(Exception ex)
            {
                Error = ex;
            }
        }

        public void Disconnect()
        {
            if (ConnectionState == ConnectionState.Open)
            {
                try
                {
                    sqlConnection.Close();
                    Error = null;
                }
                catch (Exception ex)
                {
                    Error = ex;
                }
            }
        }

        public int ExecuteNonQuery(string queryString)
        {
            int rowsAffected = 0;

            using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                rowsAffected = sqlCommand.ExecuteNonQuery();

            return rowsAffected;
        }

        public DataTable ExecuteQuery(string queryString)
        {
            DataTable dt = new DataTable();

            using (SqlCommand sqlCommand = new SqlCommand(queryString, sqlConnection))
                dt.Load(sqlCommand.ExecuteReader());

            return dt;
        }
    }
}
