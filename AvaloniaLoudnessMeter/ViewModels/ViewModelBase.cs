using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace AvaloniaLoudnessMeter.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}