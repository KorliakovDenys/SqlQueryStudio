using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SqlQueryStudio;

public class SqlController{

    private readonly SqlConnection _sqlConnection;

    private SqlDataAdapter _sqlDataAdapter;
    
    public SqlController(string connectionString){
        _sqlConnection = new SqlConnection(connectionString);
    }

    public object ExecuteCommand(string sqlCommand){
        try{
            var dataSet = new DataSet();

            _sqlDataAdapter = new SqlDataAdapter(sqlCommand, _sqlConnection);
            
            _sqlDataAdapter.Fill(dataSet);
            
            return dataSet.Tables[0];
        }
        catch(Exception exception){
            return exception.Message;
        }
    }
}