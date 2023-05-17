using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SqlQueryStudio;

public sealed class TabItemDataWrapper:INotifyPropertyChanged{
    private object _data;
    
    public object Data{
        get => _data;
        set{
            _data = value;
            OnPropertyChanged();
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null){
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}