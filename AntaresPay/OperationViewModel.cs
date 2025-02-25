using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AntaresPay;

[QueryProperty("Operation", "Operation")]
public partial class OperationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? Operation { get; set; }

    [RelayCommand]
    private static async Task Return() => await Shell.Current.GoToAsync("..");
}
