using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prism.Commands;

namespace SqlQueryStudio;

public sealed class WindowView : ViewModel{
    private DelegateCommand<Node>? _selectTableCommand;
    
    private DelegateCommand _sopenConsoleCommand;

    private DelegateCommand? _refreshDbCommand;

    private DelegateCommand<TableView>? _refreshTableCommand;

    private DelegateCommand<TableView>? _updateCommand;

    private DelegateCommand<TabItem>? _closeTabCommand;

    private DelegateCommand<ConsoleView>? _executeQueryCommand;

    private readonly SqlController _sqlController =
        new("Data Source=188.239.119.71,1433;User ID=Server;Password=qwe123;");

    public ObservableCollection<Node> DbTree{ get; private set; } = new();

    public ObservableCollection<TabItem> Tabs{ get; private set; } = new();

    public DelegateCommand<Node> SelectTableCommand =>
        _selectTableCommand ??= new DelegateCommand<Node>(ExecuteOpenTableCommand);
    
    public DelegateCommand OpenConsoleCommand =>
        _sopenConsoleCommand ??= new DelegateCommand(ExecuteOpenConsoleCommand);

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
                Content = new TabItemDataWrapper{ Data = new ConsoleView{Command = ExecuteQueryCommand} },
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
            var dataTable = _sqlController.GetTable(node.Parent.Name, node.Name);

            if (dataTable == null) return;

            var tabItemStyle = (Style)Application.Current.Resources["TabItemTableView"];

            var tabItem = new TabItem{
                Header = dataTable.TableName,
                Content = new TabItemDataWrapper{ Data = new TableView{Node = node, DataTable = dataTable, Refresh = RefreshTableCommand, Update = UpdateCommand} },
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
                    node.Nodes.Add(new Node{ Parent = node, Name = table, Command = SelectTableCommand });
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
            tableView.DataTable = _sqlController.GetTable(tableView.Node.Parent.Name, tableView.Node.Name);
        }
        catch (Exception e){
            Console.WriteLine(e);
        }
    }

    private void ExecuteUpdateCommand(TableView dataTable){
        try{
            _sqlController.UpdateTable(dataTable.DataTable);
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