
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MBModManager.Handlers;

internal sealed class IsEnabledState : INotifyPropertyChanged {

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _enabled;

    public bool Enabled {
        get => _enabled;
        set
        {
            _enabled = value;
            OnPropertyChanged();
        }
    }

    public void Set(bool newState) {
        Enabled = newState;
    }

    public IsEnabledState(bool c) {
        Enabled = c;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        var handler = PropertyChanged;

        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}