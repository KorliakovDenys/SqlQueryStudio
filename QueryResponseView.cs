using System.Data;

namespace SqlQueryStudio;

public sealed class QueryResponseView : ViewModel{
    private DataTable? _dataTable;

    private MessageHandlerArgs? _messageHandlerArgs;

    public DataTable? DataTable{
        get => _dataTable;
        set{
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public MessageHandlerArgs? MessageHandlerArgs{
        get => _messageHandlerArgs;
        set{
            _messageHandlerArgs = value;
            OnPropertyChanged();
        }
    }
}