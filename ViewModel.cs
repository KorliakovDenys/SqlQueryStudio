using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class ViewModel : INotifyPropertyChanged{
    private string _queryField;

    private string _responseText;

    private DataTable _dataTable;

    private DelegateCommand _executeCommand;

    private DelegateCommand _updateCommand;

    private readonly SqlController _sqlController =
        new ("Data Source=188.239.119.71,1433;Initial Catalog=Tea;User ID=Server;Password=qwe123;");

    public string QueryField{
        get => _queryField;
        set{
            _queryField = value;
            OnPropertyChanged();
        }
    }

    public string ResponseText{
        get => _responseText;
        set{
            _responseText = value;
            OnPropertyChanged();
        }
    }

    public DataTable DataTable{
        get => _dataTable;
        set{
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public DelegateCommand ExecuteCommand => _executeCommand ??= new DelegateCommand(ExecuteExecuteCommand);

    public DelegateCommand UpdateCommand => _updateCommand ??= new DelegateCommand(ExecuteUpdateCommand);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ExecuteExecuteCommand(){
        ClearOutputs();
        
        var response = _sqlController.ExecuteCommand(QueryField);

        switch (response){
            case DataTable dataTable:
                DataTable = dataTable;
                break;
            case string message:
                ResponseText = message;
                break;
        }
    }

    private void ExecuteUpdateCommand(){ }

    private void ClearOutputs(){
        DataTable = null;
        ResponseText = string.Empty;
    }
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}