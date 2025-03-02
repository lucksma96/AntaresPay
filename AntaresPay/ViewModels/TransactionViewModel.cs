using AntaresPay.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntaresPay.ViewModels;

[QueryProperty("Operation", "Operation"), QueryProperty("Value", "Value")]
public partial class TransactionViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? Operation { get; set; }

    [ObservableProperty]
    public partial int Value { get; set; }

    [ObservableProperty]
    public partial bool IsDeviceListening { get; set; }

    [ObservableProperty]
    public partial bool IsNfcEnabled { get; set; }

    public UnitData? UnitData { get; set; }
}
