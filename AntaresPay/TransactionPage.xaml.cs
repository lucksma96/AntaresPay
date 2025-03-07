using AntaresPay.Models;
using AntaresPay.ViewModels;
using Plugin.NFC;
using System.Text.Json;

namespace AntaresPay;

public partial class TransactionPage : ContentPage
{
    private readonly TransactionViewModel _vm;
    public const string ALERT_TITLE = "NFC";
    public const string MIME_TYPE = "application/json";
    private bool _isDeviceiOS;
    private bool _eventsAlreadySubscribed;
    private readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public TransactionPage(TransactionViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        Loaded += TransactionPage_Loaded;
    }

    protected override bool OnBackButtonPressed()
    {
        UnsubscribeEvents();
        CrossNFC.Current.StopListening();
        return base.OnBackButtonPressed();
    }

    private async void TransactionPage_Loaded(object? sender, EventArgs e)
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

            await Publish();
        }
    }

    void SubscribeEvents()
    {
        if (_eventsAlreadySubscribed)
            return;

        _eventsAlreadySubscribed = true;

        CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
        CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;

        if (_isDeviceiOS)
            CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;
    }

    void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;
        CrossNFC.Current.OnTagListeningStatusChanged -= Current_OnTagListeningStatusChanged;

        if (_isDeviceiOS)
            CrossNFC.Current.OniOSReadingSessionCancelled -= Current_OniOSReadingSessionCancelled;
    }

    void Current_OnTagListeningStatusChanged(bool isListening) => _vm.IsDeviceListening = isListening;

    void Current_OniOSReadingSessionCancelled(object? sender, EventArgs e) => Debug("iOS NFC Session has been cancelled");

    async void Current_OnMessagePublished(ITagInfo tagInfo)
    {
        try
        {
            UnsubscribeEvents();
            await ShowAlert("Sucesso!"); // TODO: (message) Writing tag operation successful
            await _vm.GoHomeCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
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
            var first = tagInfo.Records[0];

            var unitData = GetUnitData(first);
            CommitTransaction(unitData, _vm.Operation, _vm.Value);
            _vm.UnitData = unitData;

            if (_vm.UnitData is null)
                throw new ArgumentNullException();

            var payload = JsonSerializer.Serialize<UnitData>(_vm.UnitData, _serializeOptions);

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

    async Task Publish()
    {
        await StartListeningIfNotiOS();
        try
        {
            CrossNFC.Current.StartPublishing();
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    UnitData GetUnitData(NFCNdefRecord record)
    {
        var data = JsonSerializer.Deserialize<UnitData>(record.Message, _serializeOptions);
        if (data is null || !data.IsValidUnit())
            throw new KeyNotFoundException();

        return data;
    }

    void Debug(string message) => System.Diagnostics.Debug.WriteLine(message);

    Task ShowAlert(string message, string? title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? ALERT_TITLE : title, message, "OK");

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

    private void CommitTransaction(UnitData unitData, string? operation, int value)
    {
        switch (operation)
        {
            case "Pagar":
                unitData.Balance += value;
                break;
            case "Cobrar":
                unitData.Balance -= value;
                break;
            default:
                break;
        }
    }
}