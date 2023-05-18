namespace SqlQueryStudio;

public sealed class TabItemDataWrapper : ViewModel{
    private object _data;

    public object Data{
        get => _data;
        set{
            _data = value;
            OnPropertyChanged();
        }
    }
}