using System.Data;
using Prism.Commands;

namespace SqlQueryStudio;

public class TableView : ViewModel{
    private DataTable? _dataTable;

    public DataTable? DataTable{
        get => _dataTable;
        set{
            _dataTable = value;
            OnPropertyChanged();
        }
    }

    public Node Node{ get; set; }

    public DelegateCommand<TableView>? Refresh{ get; set; }

    public DelegateCommand<TableView>? Update{ get; set; }
}