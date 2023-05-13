using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class ViewModel : INotifyPropertyChanged{
    private DataTable? _dataTable;

    private DelegateCommand<string>? _selectTableCommand;

    private DelegateCommand? _refreshDbCommand;

    private DelegateCommand? _refreshTableCommand;

    private DelegateCommand? _updateCommand;

    private readonly SqlController _sqlController =
        new("***");

    public ObservableCollection<string> TableNames{ get; private set; } = new();

    public DataTable? DataTable{
        get => _dataTable;
        private set{
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public DelegateCommand<string> SelectTableCommand =>
        _selectTableCommand ??= new DelegateCommand<string>(ExecuteSelectTableCommandCommand);

    public DelegateCommand RefreshDbCommand => _refreshDbCommand ??= new DelegateCommand(ExecuteRefreshDbCommand);

    public DelegateCommand RefreshTableCommand =>
        _refreshTableCommand ??= new DelegateCommand(ExecuteRefreshTableCommand);

    public DelegateCommand UpdateCommand => _updateCommand ??= new DelegateCommand(ExecuteUpdateCommand);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ExecuteSelectTableCommandCommand(string tableName){
        DataTable = _sqlController.GetTable(tableName);
    }

    private void ExecuteRefreshDbCommand(){
        TableNames.Clear();

        var tables = _sqlController.GetTableNames();

        var names = tables.ToList();

        foreach (var name in names){
            TableNames.Add(name);
        }
    }

    private void ExecuteRefreshTableCommand(){
        if (DataTable == null) return;
        
        ExecuteSelectTableCommandCommand(DataTable.TableName);
    }

    private void ExecuteUpdateCommand(){
        if (DataTable == null) return;
        
        _sqlController.UpdateTable(DataTable);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}