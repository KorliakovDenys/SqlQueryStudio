using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace SqlQueryStudio;

public sealed class QueryResponse : ViewModel{
    
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