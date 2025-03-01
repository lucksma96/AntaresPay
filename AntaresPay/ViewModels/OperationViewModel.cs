using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AntaresPay;

[QueryProperty("Operation", "Operation")]
public partial class OperationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Operation { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(BackspaceCommand), nameof(RegisterInputCommand))]
    public partial int Value { get; set; }

    [RelayCommand]
    private static async Task Return() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task GoToTransaction() => await Shell.Current.GoToAsync($"{nameof(TransactionPage)}?{nameof(Operation)}={Operation}&{nameof(Value)}={Value}");

    [RelayCommand(CanExecute = nameof(IsValueLessThanMaxLength))]
    private void RegisterInput(string input)
    {
        _ = int.TryParse($"{Value}{input}", out int value);
        Value = value;
    }

    [RelayCommand]
    private void Clear() => Value = 0;

    [RelayCommand(CanExecute = nameof(IsValueNotEmpty))]
    private void Backspace()
    {
        var value = Value.ToString();
        _ = int.TryParse(value[..^1], out int newValue);
        Value = newValue;
    }

    private bool IsValueNotEmpty()
    {
        return Value != 0;
    }

    private bool IsValueLessThanMaxLength()
    {
        return Value.ToString().Length < 7;
    }
}
