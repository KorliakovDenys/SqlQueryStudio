using Prism.Commands;

namespace SqlQueryStudio;

public class ConsoleView : ViewModel{
    private string _queryInput;

    private QueryResponseView? _queryResponse;

    private DelegateCommand<ConsoleView>? _command;

    public string QueryInput{
        get => _queryInput;
        set{
            _queryInput = value;
            OnPropertyChanged();
        }
    }

    public QueryResponseView? QueryResponse{
        get => _queryResponse;
        set{
            _queryResponse = value;
            OnPropertyChanged();
        }
    }

    public DelegateCommand<ConsoleView>? Command{
        get => _command;
        set{
            _command = value;
            OnPropertyChanged();
        }
    }
}