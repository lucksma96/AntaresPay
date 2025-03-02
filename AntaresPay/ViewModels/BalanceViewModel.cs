using AntaresPay.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntaresPay.ViewModels;

public partial class BalanceViewModel : ObservableObject
{
    [ObservableProperty]
    public partial UnitData UnitData { get; set; }

    [ObservableProperty]
    public partial bool IsDeviceListening { get; set; }

    [ObservableProperty]
    public partial bool IsNfcEnabled { get; set; }
}
