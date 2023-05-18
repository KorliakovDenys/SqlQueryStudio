using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class WindowView : ViewModel{
    private DelegateCommand<Node>? _selectTableCommand;

    private DelegateCommand _openConsoleCommand;

    private DelegateCommand? _refreshDbCommand;

    private DelegateCommand<TableView>? _refreshTableCommand;

    private DelegateCommand<TableView>? _updateCommand;

    private DelegateCommand<TabItem>? _closeTabCommand;

    private DelegateCommand<ConsoleView>? _executeQueryCommand;

    private readonly SqlController _sqlController =
        new(new SqlConnectionData("188.239.119.71,1433","Server", "qwe123"));
    public ObservableCollection<Node> DbTree{ get; private set; } = new();

    public ObservableCollection<TabItem> Tabs{ get; private set; } = new();

    public DelegateCommand<Node> SelectTableCommand =>
        _selectTableCommand ??= new DelegateCommand<Node>(ExecuteOpenTableCommand);

    public DelegateCommand OpenConsoleCommand =>
        _openConsoleCommand ??= new DelegateCommand(ExecuteOpenConsoleCommand);

    public DelegateCommand RefreshDbCommand => _refreshDbCommand ??= new DelegateCommand(ExecuteRefreshDbCommand);

    public DelegateCommand<TableView> RefreshTableCommand =>
        _refreshTableCommand ??= new DelegateCommand<TableView>(ExecuteRefreshTableCommand);

    public DelegateCommand<TableView> UpdateCommand =>
        _updateCommand ??= new DelegateCommand<TableView>(ExecuteUpdateCommand);

    public DelegateCommand<TabItem> CloseTabCommand =>
        _closeTabCommand ??= new DelegateCommand<TabItem>(ExecuteCloseTabCommand);

    public DelegateCommand<ConsoleView> ExecuteQueryCommand =>
        _executeQueryCommand ??= new DelegateCommand<ConsoleView>(ExecuteExecuteQueryCommand);


    private void ExecuteOpenConsoleCommand(){
        try{
            var tabItemStyle = (Style)Application.Current.Resources["TabItemConsoleView"];

            var tabItem = new TabItem{
                Header = "Console",
                Content = new TabItemDataWrapper{ Data = new ConsoleView{ Command = ExecuteQueryCommand } },
                IsSelected = true,
                Style = tabItemStyle
            };

            Tabs.Add(tabItem);
        }
        catch (Exception e){
            Debug.WriteLine(e);
        }
    }

    private void ExecuteOpenTableCommand(Node node){
        try{
            SqlDataAdapter? sqlDataAdapter = null;

            var dataTable = _sqlController.GetTable(node.Parent.Name, node.Name, ref sqlDataAdapter);

            if (dataTable == null) return;

            var tabItemStyle = (Style)Application.Current.Resources["TabItemTableView"];

            var tabItem = new TabItem{
                Header = dataTable.TableName,
                Content = new TabItemDataWrapper{
                    Data = new TableView{
                        Node = node,
                        DataTable = dataTable,
                        Refresh = RefreshTableCommand,
                        Update = UpdateCommand,
                        SqlDataAdapter = sqlDataAdapter
                    }
                },
                IsSelected = true,
                Style = tabItemStyle
            };

            Tabs.Add(tabItem);
        }
        catch (Exception e){
            Debug.WriteLine(e);
        }
    }

    private void ExecuteRefreshDbCommand(){
        try{
            DbTree.Clear();

            var databases = _sqlController.GetDatabases();

            foreach (var database in databases){
                var node = new Node{ Name = database, Nodes = new ObservableCollection<Node>() };
                var tables = _sqlController.GetTableNames(database);
                foreach (var table in tables){
                    node.Nodes.Add(new Node{
                        Parent = node, 
                        Name = table, 
                        Command = SelectTableCommand
                    });
                }

                DbTree.Add(node);
            }
        }
        catch (Exception e){
            Debug.WriteLine(e);
        }
    }

    private void ExecuteRefreshTableCommand(TableView tableView){
        try{
            var dbName = tableView.Node.Parent.Name; 
            var tableName = tableView.Node.Name;
            var tableViewSqlDataAdapter = tableView.SqlDataAdapter;
            tableView.DataTable = _sqlController.GetTable(dbName, tableName, ref tableViewSqlDataAdapter);
        }
        catch (Exception e){
            Console.WriteLine(e);
        }
    }

    private void ExecuteUpdateCommand(TableView tableView){
        try{ 
            if (tableView.SqlDataAdapter == null) throw new NullReferenceException();
            if (tableView.DataTable == null) throw new NullReferenceException();

            _sqlController.UpdateTable(tableView.DataTable, tableView.SqlDataAdapter);
        }
        catch (Exception e){
            Console.WriteLine(e);
        }
    }

    private void ExecuteCloseTabCommand(TabItem tabItem){
        try{
            Tabs.Remove(tabItem);
        }
        catch (Exception exception){
            Debug.WriteLine(exception.Message);
        }
    }

    private void ExecuteExecuteQueryCommand(ConsoleView consoleView){
        try{
            var response = _sqlController.Fetch(consoleView.QueryInput).Result;

            consoleView.QueryResponse = response;
        }
        catch (Exception exception){
            Debug.WriteLine(exception.Message);
        }
    }
}