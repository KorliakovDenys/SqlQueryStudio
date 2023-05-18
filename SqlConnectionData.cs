using System.Dynamic;

namespace SqlQueryStudio;

public class SqlConnectionData{
    public SqlConnectionData(string dataSource, string userId, string password){
        DataSource = dataSource;
        UserId = userId;
        Password = password;
    }
    public string DataSource{ get; set; }
    public string UserId{ get; set; }
    public string Password{ get; set; }

    public string Get(){
        return $"Data Source={DataSource};User ID={UserId};Password={Password};";
    }

    public string Get(string dbName){
        return $"Data Source={DataSource};Initial Catalog={dbName};User ID={UserId};Password={Password};";
    }
}