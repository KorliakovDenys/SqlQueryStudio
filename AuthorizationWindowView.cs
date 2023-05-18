using Prism.Commands;

namespace SqlQueryStudio;

public class AuthorizationWindowView : ViewModel{
    private string _ip;

    private string _port;

    private string _userId;

    private string _password;

    public string Ip{
        get => _ip;
        set{
            _ip = value;
            OnPropertyChanged();
        }
    }

    public string Port{
        get => _port;
        set{
            _port = value;
            OnPropertyChanged();
        }
    }

    public string UserId{
        get => _userId;
        set{
            _userId = value;
            OnPropertyChanged();
        }
    }

    public string Password{
        get => _password;
        set{
            _password = value;
            OnPropertyChanged();
        }
    }

    public SqlConnectionData GetConnectionData(){
        return new SqlConnectionData($"{Ip},{Port}", UserId, Password);
    }
}