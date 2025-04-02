using CommunityToolkit.Mvvm.ComponentModel;

namespace AntaresPay.ViewModels;

public partial class ConfigViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Theme { get; set; }
}
