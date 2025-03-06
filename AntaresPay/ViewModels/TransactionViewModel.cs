using AntaresPay.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AntaresPay.ViewModels;

[QueryProperty("Operation", "Operation"), QueryProperty("Value", "Value")]
public partial class TransactionViewModel : ObservableObject
{
    public UnitData? UnitData { get; set; }

    [ObservableProperty]
    public partial string? Operation { get; set; }

    [ObservableProperty]
    public partial int Value { get; set; }

    [ObservableProperty]
    public partial bool IsDeviceListening { get; set; }

    [ObservableProperty]
    public partial bool IsNfcEnabled { get; set; }

    [RelayCommand]
    private static async Task Return() => await Shell.Current.GoToAsync("..");
}
