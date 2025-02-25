namespace AntaresPay
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(OperationPage), typeof(OperationPage));
        }
    }
}
