using AntaresPay.Models;
using AntaresPay.ViewModels;
using Plugin.NFC;
using System.Text.Json;

namespace AntaresPay;

public partial class BalancePage : ContentPage
{
    private const string ALERT_TITLE = "NFC";

    private readonly BalanceViewModel _vm;
    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    private bool _isDeviceiOS;

    public BalancePage(BalanceViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Loaded += BalancePage_Loaded;
    }

    protected override bool OnBackButtonPressed()
    {
        UnsubscribeEvents();
        CrossNFC.Current.StopListening();
        return base.OnBackButtonPressed();
    }

    private void SubscribeEvents()
    {
        CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
    }

    private void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
    }

    private async void BalancePage_Loaded(object? sender, EventArgs e)
    {
        // In order to support Mifare Classic 1K tags (read/write), legacy mode must be true.
        CrossNFC.Legacy = true;

        if (CrossNFC.IsSupported)
        {
            if (!CrossNFC.Current.IsAvailable)
                await ShowAlert("NFC não está disponível"); // TODO: (message) NFC is not available

            _vm.IsNfcEnabled = CrossNFC.Current.IsEnabled;
            if (!_vm.IsNfcEnabled)
                await ShowAlert("NFC está desabilitada"); // TODO: (message) NFC is disabled

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                _isDeviceiOS = true;

            SubscribeEvents();

            await StartListeningIfNotiOS();
        }
    }
    async void Current_OnMessageReceived(ITagInfo tagInfo)
    {
        if (tagInfo == null)
        {
            await ShowAlert("Cartão não detectado"); // TODO: (message) No tag found
            return;
        }

        // Customized serial number
        var identifier = tagInfo.Identifier;
        _ = NFCUtils.ByteArrayToHexString(identifier, ":");

        var first = tagInfo.Records[0];
        _vm.UnitData = ParseUnitData(first);
        await _vm.LoadOperationsCommand.ExecuteAsync(_vm.UnitData.Name);
    }

    async Task StartListeningIfNotiOS()
    {
        if (_isDeviceiOS)
        {
            SubscribeEvents();
            return;
        }
        await BeginListening();
    }

    async Task BeginListening()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SubscribeEvents();
                CrossNFC.Current.StartListening();
            });
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    async Task StopListening()
    {
        try
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CrossNFC.Current.StopListening();
                UnsubscribeEvents();
            });
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    Task ShowAlert(string message, string? title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? ALERT_TITLE : title, message, "OK");

    UnitData ParseUnitData(NFCNdefRecord record)
    {
        var data = JsonSerializer.Deserialize<UnitData>(record.Message, _serializeOptions);
        if (data is null || !data.IsValidUnit())
            throw new KeyNotFoundException();

        return data;
    }
}