using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AntaresPay;

[QueryProperty("Operation", "Operation")]
public partial class OperationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Operation { get; set; }

    [ObservableProperty]
    public partial int Value { get; set; }

    [RelayCommand]
    private static async Task Return() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task GoToTransaction() => await Shell.Current.GoToAsync($"{nameof(TransactionPage)}?{nameof(Operation)}={Operation}");
}
