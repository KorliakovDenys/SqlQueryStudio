using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class ViewModel : INotifyPropertyChanged {
    private DataTable? _dataTable;

    private DelegateCommand<Node>? _selectTableCommand;

    private DelegateCommand? _refreshDbCommand;

    private DelegateCommand? _refreshTableCommand;

    private DelegateCommand? _updateCommand;

    private readonly SqlController _sqlController =
        new("***");

    public ObservableCollection<Node> TableNames { get; private set; } = new();

    public DataTable? DataTable {
        get => _dataTable;
        private set {
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public DelegateCommand<Node> SelectTableCommand =>
        _selectTableCommand ??= new DelegateCommand<Node>(ExecuteSelectTableCommandCommand);

    public DelegateCommand RefreshDbCommand => _refreshDbCommand ??= new DelegateCommand(ExecuteRefreshDbCommand);

    public DelegateCommand RefreshTableCommand =>
        _refreshTableCommand ??= new DelegateCommand(ExecuteRefreshTableCommand);

    public DelegateCommand UpdateCommand => _updateCommand ??= new DelegateCommand(ExecuteUpdateCommand);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ExecuteSelectTableCommandCommand(Node node) {
        Debug.WriteLine("Table name: " + node.Name);
        DataTable = _sqlController.GetTable(node.Name);
    }

    private void ExecuteRefreshDbCommand() {
        TableNames.Clear();

 

        //var names = tables.ToList();

        //foreach (var name in names) {
        //    TableNames.Add(new Node { Name = name, Nodes = new ObservableCollection<Node> { new Node { Name = name, Command = SelectTableCommand } } });
        //}
    }

    private void ExecuteRefreshTableCommand() {
        if (DataTable == null) return;

        //!!!ExecuteSelectTableCommandCommand(DataTable.TableName);
    }

    private void ExecuteUpdateCommand() {
        if (DataTable == null) return;

        _sqlController.UpdateTable(DataTable);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}