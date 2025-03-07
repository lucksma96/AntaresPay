using AntaresPay.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AntaresPay.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    public RegisterViewModel()
    {
        Units = [
            // masculinas
            new UnitData() { Id = "phoenix", Name = "Phoenix" },
            new UnitData() { Id = "scorpion", Name = "Scorpion" },
            new UnitData() { Id = "andromeda", Name = "Andrômeda" },
            new UnitData() { Id = "europa", Name = "Europa" },
            new UnitData() { Id = "hercules", Name = "Hércules" },
            // femininas
            new UnitData() { Id = "alpha", Name = "Alpha" },
            new UnitData() { Id = "lyra", Name = "Lyra" },
            new UnitData() { Id = "shaula", Name = "Shaula" },
            new UnitData() { Id = "bellatrix", Name = "Bellatrix" },
            new UnitData() { Id = "pegasus", Name = "Pégasus" },
        ];
    }

    [ObservableProperty]
    public partial List<UnitData> Units { get; set; }

    [ObservableProperty]
    public partial UnitData UnitData { get; set; }

    [ObservableProperty]
    public partial bool IsDevicePublishing { get; set; }

    [ObservableProperty]
    public partial bool IsNfcEnabled { get; set; }
}
