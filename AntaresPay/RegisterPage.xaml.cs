using AntaresPay.Models;
using AntaresPay.ViewModels;
using CommunityToolkit.Mvvm.Input;
using Plugin.NFC;
using System.Text.Json;

namespace AntaresPay;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _vm;
    public const string ALERT_TITLE = "NFC";
    public const string MIME_TYPE = "application/json";
    private bool _isDeviceiOS;
    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
		BindingContext =_vm = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        UnsubscribeEvents();
        CrossNFC.Current.StopPublishing();
        return base.OnBackButtonPressed();
    }

    private async void Confirm_Clicked(object sender, EventArgs args)
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

            try
            {
                SubscribeEvents();
                await Publish();
            }
            catch (Exception ex)
            {
                await ShowAlert(ex.Message);
            }
        }
    }

    private void Cancel_Clicked(object sender, EventArgs args)
    {
        UnsubscribeEvents();
        CrossNFC.Current.StopPublishing();
        _vm.IsDevicePublishing = false;
    }

    void SubscribeEvents()
    {
        CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
    }

    void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;
    }

    async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
    {
        if (!CrossNFC.Current.IsWritingTagSupported)
        {
            await ShowAlert("Este dispositivo não permite esta operação"); // TODO: (message) Writing tag is not supported on this device
            return;
        }

        try
        {
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");

            var payload = JsonSerializer.Serialize(_vm.UnitData, _serializeOptions);

            var record = new NFCNdefRecord
            {
                TypeFormat = NFCNdefTypeFormat.Mime,
                MimeType = MIME_TYPE,
                Payload = NFCUtils.EncodeToByteArray(payload)
            };

            tagInfo.Records = [record];

            CrossNFC.Current.PublishMessage(tagInfo);
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    async void Current_OnMessagePublished(ITagInfo tagInfo)
    {
        try
        {
            CrossNFC.Current.StopPublishing();
            _vm.IsDevicePublishing = false;
            await ShowAlert("Gravação concluída");
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    async Task StartListeningIfNotiOS()
    {
        if (_isDeviceiOS)
            return;
        await BeginListening();
    }

    async Task BeginListening()
    {
        try
        {
            CrossNFC.Current.StartListening();
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    async Task Publish()
    {
        await StartListeningIfNotiOS();
        _vm.IsDevicePublishing = true;
        try
        {
            CrossNFC.Current.StartPublishing();
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    Task ShowAlert(string message, string? title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? ALERT_TITLE : title, message, "OK");
}