using AntaresPay.ViewModels;

namespace AntaresPay;

public partial class AllOperationsPage : ContentPage
{
    private readonly AllOperationsViewModel _vm;

    public AllOperationsPage(AllOperationsViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
		Loaded += AllOperationsPage_Loaded;
    }

    private async void AllOperationsPage_Loaded(object? sender, EventArgs e)
    {
        await _vm.LoadOperationsCommand.ExecuteAsync(null);
    }
}