using AntaresPay.Models;
using AntaresPay.Persistence.Entities;
using AntaresPay.Persistence.Repositories;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AntaresPay.ViewModels;

public partial class BalanceViewModel(OperationRepository operationRepository) : ObservableObject
{
    private readonly OperationRepository _operationRepository = operationRepository;

    [ObservableProperty]
    public partial UnitData? UnitData { get; set; }

    [ObservableProperty]
    public partial bool IsDeviceListening { get; set; }

    [ObservableProperty]
    public partial bool IsNfcEnabled { get; set; }

    [RelayCommand]
    private static async Task Return() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task LoadOperations(string unitName) => Operations = (await _operationRepository.GetByUnitNameAsync(unitName)).ToObservableCollection();

    [ObservableProperty]
    public partial ObservableCollection<OperationEntity> Operations { get; set; } = [];
}
