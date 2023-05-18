using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryStudio;

public class SqlController{
    private readonly SqlConnectionData _connectionData;

    public static bool TestConnection(SqlConnectionData connectionData){
        try{
            using var connection = new SqlConnection(connectionData.Get());

            connection.Open();
        }
        catch (Exception){
            return false;
        }
        return true;
    }

    public SqlController(SqlConnectionData connectionData){
        _connectionData = connectionData;
    }

    public IEnumerable<string> GetDatabases(){
        var dataBasesList = new List<string>();

        try{
            using var connection = new SqlConnection(_connectionData.Get());

            var command = new SqlCommand("SELECT name FROM sys.databases", connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read()){
                var databaseName = reader["name"].ToString();

                if (databaseName != null) dataBasesList.Add(databaseName);
            }
        }
        catch (Exception exception){
            Debug.WriteLine(exception.Message);
        }

        return dataBasesList;
    }

    public IEnumerable<string> GetTableNames(string dbName){
        var tablesList = new List<string>();

        try{
            var adapter =
                new SqlDataAdapter(
                    $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';",
                    _connectionData.Get(dbName));

            var dataTable = new DataTable();

            adapter.Fill(dataTable);

            tablesList = (from DataRow dataRow in dataTable.Rows select (string)dataRow["TABLE_NAME"]).ToList();
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }

        return tablesList;
    }

    public DataTable? GetTable(string dbName, string tableName, ref SqlDataAdapter? sqlDataAdapter){
        DataTable? dataTable = null;

        try{
            sqlDataAdapter ??= new SqlDataAdapter($"SELECT * FROM {tableName}", _connectionData.Get(dbName));

            dataTable = new DataTable(tableName);

            sqlDataAdapter.Fill(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }

        return dataTable;
    }

    public void UpdateTable(DataTable dataTable, SqlDataAdapter sqlDataAdapter){
        try{
            if (dataTable == null) throw new NullReferenceException("dataTable can not be null");
            if (sqlDataAdapter == null) throw new NullReferenceException("dataTable can not be null");

            var commandBuilder = new SqlCommandBuilder(sqlDataAdapter);

            sqlDataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();

            sqlDataAdapter.Update(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }
    }

    public async Task<QueryResponseView> Fetch(string sqlCommand){
        var dataTable = new DataTable();
        MessageHandlerArgs? message = null;
        try{
            await using var connection = new SqlConnection(_connectionData.Get());

            await connection.OpenAsync();

            var command = new SqlCommand(sqlCommand, connection);
            var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);


            var recAff = reader.RecordsAffected;

            if (!reader.HasRows){
                dataTable = null;
                message = new MessageHandlerArgs($"Records affected: {recAff}", MessageHandlerArgs.Type.Information);
                throw new Exception();
            }

            for (var i = 0; i < reader.FieldCount; i++){
                dataTable?.Columns.Add(reader.GetName(i));
            }

            while (reader.Read()){
                var row = dataTable?.NewRow();

                for (var i = 0; i < reader.FieldCount; i++){
                    row![i] = reader.GetValue(i);
                }

                dataTable?.Rows.Add(row!);
            }
        }
        catch (SqlException exception){
            message = new MessageHandlerArgs($"{exception.Message}", MessageHandlerArgs.Type.Error);
        }
        catch (Exception exception){
            Debug.WriteLine(exception.Message);
        }

        return new QueryResponseView{ DataTable = dataTable, MessageHandlerArgs = message };
    }
}