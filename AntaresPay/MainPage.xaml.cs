
namespace AntaresPay
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OperationButtons_Clicked(object sender, EventArgs args)
        {
            var operation = ((Button)sender).Text;

            await Shell.Current.GoToAsync($"{nameof(OperationPage)}?Operation={operation}");
        }

        async void RegisterButton_Clicked(object sender, EventArgs args)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}