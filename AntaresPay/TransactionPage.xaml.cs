using AntaresPay.ViewModels;

namespace AntaresPay;

public partial class TransactionPage : ContentPage
{
	public TransactionPage(TransactionViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}