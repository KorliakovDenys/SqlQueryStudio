using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class ViewModel : INotifyPropertyChanged {
    
    private DataTable? _dataTable;

    private DelegateCommand<Node>? _selectTableCommand;

    private DelegateCommand? _refreshDbCommand;

    private DelegateCommand? _refreshTableCommand;

    private DelegateCommand? _updateCommand;
    
    private DelegateCommand<TabItem>? _closeTabCommand;

    private readonly SqlController _sqlController =
        new("***");

    public ObservableCollection<Node> DbTree { get; private set; } = new();
    
    public ObservableCollection<TabItem> Tabs { get; private set; } = new();

    public DataTable? DataTable {
        get => _dataTable;
        private set {
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public DelegateCommand<Node> SelectTableCommand => _selectTableCommand ??= new DelegateCommand<Node>(ExecuteSelectTableCommandCommand);

    public DelegateCommand RefreshDbCommand => _refreshDbCommand ??= new DelegateCommand(ExecuteRefreshDbCommand);

    public DelegateCommand RefreshTableCommand => _refreshTableCommand ??= new DelegateCommand(ExecuteRefreshTableCommand);

    public DelegateCommand UpdateCommand => _updateCommand ??= new DelegateCommand(ExecuteUpdateCommand);
    
    public DelegateCommand<TabItem> CloseTabCommand => _closeTabCommand ??= new DelegateCommand<TabItem>(ExecuteCloseTabCommand);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ExecuteSelectTableCommandCommand(Node node) {
        Debug.WriteLine("Data: " + node.Parent.Name + node.Name);

        var dataTable = _sqlController.GetTable(node.Parent.Name, node.Name);

        if (dataTable == null) return;
        
        var dataGrid = new DataGrid{
            ItemsSource = dataTable.DefaultView
        };

        var tabItem = new TabItem{
            Header = dataTable.TableName,
            Content = dataGrid,
            IsSelected = true
        };
        
        Tabs.Add(tabItem);
    }

    private void ExecuteRefreshDbCommand() {
        DbTree.Clear();

        var databases = _sqlController.GetDatabases();

        foreach (var database in databases){
            var node = new Node{ Name = database, Nodes = new ObservableCollection<Node>()};
            var tables = _sqlController.GetTableNames(database);
            foreach (var table in tables){
                node.Nodes.Add(new Node{Parent = node, Name = table, Command = SelectTableCommand});
            }
            DbTree.Add(node);
        }

    }

    private void ExecuteRefreshTableCommand() {
        if (DataTable == null) return;

        //!!!ExecuteSelectTableCommandCommand(DataTable.TableName);
    }

    private void ExecuteUpdateCommand() {
        if (DataTable == null) return;

        _sqlController.UpdateTable(DataTable);
    }

    private void ExecuteCloseTabCommand(TabItem tabItem){
        try{
            Tabs.Remove(tabItem);
        }
        catch (Exception exception){
            Debug.WriteLine(exception.Message);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}