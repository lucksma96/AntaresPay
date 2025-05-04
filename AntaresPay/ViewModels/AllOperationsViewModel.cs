using AntaresPay.Persistence.Entities;
using AntaresPay.Persistence.Repositories;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AntaresPay.ViewModels;

public partial class AllOperationsViewModel(OperationRepository operationRepository) : ObservableObject
{
    private readonly OperationRepository _operationRepository = operationRepository;

    [RelayCommand]
    private async Task LoadOperations() => Operations = (await _operationRepository.GetAllAsync()).OrderByDescending(x => x.CreatedAt).ToObservableCollection();

    [ObservableProperty]
    public partial ObservableCollection<OperationEntity> Operations { get; set; } = [];
}
