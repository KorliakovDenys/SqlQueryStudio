using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        finally{
            _connection.Close();
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
            var adapter = new SqlDataAdapter($"USE {dbName} SELECT * FROM {tableName}", _connection);

            dataTable = new DataTable(tableName);

            adapter.Fill(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }

        return dataTable;
    }

    public void UpdateTable(DataTable? dataTable){
        try{
            var adapter = new SqlDataAdapter();
            
            _ = new SqlCommandBuilder(adapter);

            adapter.Update(dataTable);
        }
        catch (Exception e){
            Debug.WriteLine(e.Message);
        }
    }

    public async Task<QueryResponse> Fetch(string sqlCommand){
        var dataTable = new DataTable();
        MessageHandlerArgs? message = null;
        try{
            await _connection.OpenAsync();

            var command = new SqlCommand(sqlCommand, _connection);
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
        catch (Exception){ }
        finally{
            await _connection.CloseAsync();
        }

        return new QueryResponse{ DataTable = dataTable, MessageHandlerArgs = message };
    }
}