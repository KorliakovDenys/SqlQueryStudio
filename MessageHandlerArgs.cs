using System;

namespace SqlQueryStudio;

public sealed class MessageHandlerArgs : ViewModel{
    public enum Type{
        Information,
        Error
    }

    private string? _message;

    private string? _color;

    private DateTime _time;

    public string? Message{
        get => _message;
        private set{
            _message = value;
            OnPropertyChanged();
        }
    }

    public string? Color{
        get => _color;
        private set{
            _color = value;
            OnPropertyChanged();
        }
    }

    public DateTime Time{
        get => _time;
        private set{
            _time = value;
            OnPropertyChanged();
        }
    }

    public MessageHandlerArgs(string? message, Type type){
        Message = message;
        Time = DateTime.Now;
        Color = type switch{
            Type.Information => "WhiteSmoke",
            Type.Error => "Red",
            _ => "White"
        };
    }
}