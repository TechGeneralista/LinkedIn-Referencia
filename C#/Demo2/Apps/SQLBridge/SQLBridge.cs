using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SQLBridgeApp
{
    public class SQLBridge
    {
        SocketTCPServer socketTCPServer;
        Dictionary<string, string> connections = new Dictionary<string, string>();


        public SQLBridge(IPAddress ipAddress, int portNumber)
        {
            socketTCPServer = new SocketTCPServer(ipAddress, portNumber);
            socketTCPServer.CreateConnectionRequest += SocketTCPServer_CreateConnectionRequest;
            socketTCPServer.TestConnectionRequest += SocketTCPServer_TestConnectionRequest;
            socketTCPServer.QueryExecuteRequest += SocketTCPServer_QueryExecuteRequest;
        }

        private void SocketTCPServer_QueryExecuteRequest(Socket client, string receivedData)
        {
            string[] splittedData = receivedData.Split(Constants.MessageSeparator);

            if (splittedData.Length != 2 || !connections.ContainsKey(splittedData[0]))
            {
                socketTCPServer.SendMessage(client, Constants.Error);
                return;
            }

            try
            {
                DataTable dataTable = new DataTable();

                using (var conn = new SqlConnection(connections[splittedData[0]]))
                using (var command = new SqlCommand(splittedData[1], conn))
                using (var sqlDataAdapter = new SqlDataAdapter(command))
                {
                    conn.Open();
                    sqlDataAdapter.Fill(dataTable);
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(Constants.Ok);
                stringBuilder.Append(Constants.MessageSeparator);

                for (int y = 0; y < dataTable.Rows.Count; y++)
                {
                    for (int x = 0; x < dataTable.Columns.Count; x++)
                    {
                        string data = dataTable.Rows[y][x].ToString();

                        stringBuilder.Append(y);
                        stringBuilder.Append('.');
                        stringBuilder.Append(x);
                        stringBuilder.Append('.');
                        stringBuilder.Append(data.Length);
                        stringBuilder.Append('>');
                        stringBuilder.Append(data);

                        if(!(y == (dataTable.Rows.Count-1) && x == (dataTable.Columns.Count - 1)))
                            stringBuilder.Append('/');
                    }
                }

                socketTCPServer.SendMessage(client, stringBuilder.ToString());
            }

            catch (Exception ex)
            {
                socketTCPServer.SendMessage(client, Constants.Error);
            }
        }

        private void SocketTCPServer_TestConnectionRequest(Socket client, string receivedData)
        {
            if(string.IsNullOrEmpty(receivedData) || !connections.ContainsKey(receivedData))
            {
                socketTCPServer.SendMessage(client, Constants.Error);
                return;
            }

            try
            {
                using(SqlConnection sqlConnection = new SqlConnection(connections[receivedData]))
                {
                    sqlConnection.Open();
                }

                socketTCPServer.SendMessage(client, Constants.Ok);
            }

            catch(Exception ex)
            {
                socketTCPServer.SendMessage(client, Constants.Error);
            }
        }

        private void SocketTCPServer_CreateConnectionRequest(Socket client, string receivedData)
        {
            string[] splittedData = receivedData.Split(Constants.MessageSeparator);

            if(splittedData.Length != 4)
            {
                socketTCPServer.SendMessage(client, Constants.Error);
                return;
            }

            string guid = Guid.NewGuid().ToString().ToUpper().Replace("-", string.Empty).Substring(0, 8);
            string connectionString = "Server=" + splittedData[0] + ";Database=" + splittedData[1] + ";User Id=" + splittedData[2] + ";Password=" + splittedData[3] + ";";

            connections[guid] = connectionString;
            socketTCPServer.SendMessage(client, Constants.Concat(Constants.Ok, guid));
        }

        public void Start()
        {
            socketTCPServer.Start();
        }
    }
}