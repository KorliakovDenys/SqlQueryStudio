using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace SqlQueryStudio;

public class SqlController{
    private readonly SqlConnection _connection;

    private SqlDataAdapter? _adapter;

    public SqlController(string connectionString){
        _connection = new SqlConnection(connectionString);
    }

    public IEnumerable<string> GetDatabases(){
        var dataBasesList = new List<string>();

        try{
            _connection.Open();
            var command = new SqlCommand("SELECT name FROM sys.databases", _connection);
            var reader = command.ExecuteReader();

            while (reader.Read()){
                var databaseName = reader["name"].ToString();
                
                if (databaseName != null) dataBasesList.Add(databaseName);
            }

            reader.Close();
        }
        catch (Exception ex){
            Debug.WriteLine(ex.Message);
        }

        return dataBasesList;
    }

    public IEnumerable<string> GetTableNames(string dbName){
        var tablesList = new List<string>();

        try{
            var adapter =
                new SqlDataAdapter(
                    $"USE {dbName} SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';",
                    _connection);

            var dataTable = new DataTable();

            adapter.Fill(dataTable);

            tablesList = (from DataRow dataRow in dataTable.Rows select (string)dataRow["TABLE_NAME"]).ToList();
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }

        return tablesList;
    }

    public DataTable? GetTable(string dbName, string tableName){
        DataTable? dataTable = null;

        try{
            _adapter = new SqlDataAdapter($"USE {dbName} SELECT * FROM {tableName}", _connection);

            dataTable = new DataTable(tableName);

            _adapter.Fill(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }

        return dataTable;
    }

    public void UpdateTable(DataTable dataTable){
        try{
            if (_adapter == null) return;

            _ = new SqlCommandBuilder(_adapter);

            _adapter.Update(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }
    }
}