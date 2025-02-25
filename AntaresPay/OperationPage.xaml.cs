namespace AntaresPay;

public partial class OperationPage : ContentPage
{
	public OperationPage(OperationViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}