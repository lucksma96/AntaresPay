using AntaresPay.Models;
using AntaresPay.ViewModels;
using Plugin.NFC;
using System.Text;
using System.Text.Json;

namespace AntaresPay;

public partial class TransactionPage : ContentPage
{
    private readonly TransactionViewModel _vm;
    public const string ALERT_TITLE = "NFC";
    public const string MIME_TYPE = "application/json";
    private NFCNdefTypeFormat _type;
    private bool _isDeviceiOS;
    private bool _eventsAlreadySubscribed;
    private JsonSerializerOptions _serializeOptions = new()
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

    /// <summary>
    /// Subscribe to the NFC events
    /// </summary>
    void SubscribeEvents()
    {
        if (_eventsAlreadySubscribed)
            return;

        _eventsAlreadySubscribed = true;

        CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
        CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
        CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
        CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;

        if (_isDeviceiOS)
            CrossNFC.Current.OniOSReadingSessionCancelled += Current_OniOSReadingSessionCancelled;
    }

    /// <summary>
    /// Unsubscribe from the NFC events
    /// </summary>
    void UnsubscribeEvents()
    {
        CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
        CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
        CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;
        CrossNFC.Current.OnNfcStatusChanged -= Current_OnNfcStatusChanged;
        CrossNFC.Current.OnTagListeningStatusChanged -= Current_OnTagListeningStatusChanged;

        if (_isDeviceiOS)
            CrossNFC.Current.OniOSReadingSessionCancelled -= Current_OniOSReadingSessionCancelled;
    }

    /// <summary>
    /// Event raised when Listener Status has changed
    /// </summary>
    /// <param name="isListening"></param>
    void Current_OnTagListeningStatusChanged(bool isListening) => _vm.IsDeviceListening = isListening;

    /// <summary>
    /// Event raised when NFC Status has changed
    /// </summary>
    /// <param name="isEnabled">NFC status</param>
    async void Current_OnNfcStatusChanged(bool isEnabled)
    {
        _vm.IsNfcEnabled = isEnabled;
        await ShowAlert($"NFC has been {(isEnabled ? "enabled" : "disabled")}");
    }

    /// <summary>
    /// Event raised when a NDEF message is received
    /// </summary>
    /// <param name="tagInfo">Received <see cref="ITagInfo"/></param>
    async void Current_OnMessageReceived(ITagInfo tagInfo)
    {
        if (tagInfo == null)
        {
            await ShowAlert("No tag found");
            return;
        }

        // Customized serial number
        var identifier = tagInfo.Identifier;
        var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
        var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

        if (!tagInfo.IsSupported)
        {
            await ShowAlert("Unsupported tag (app)", title);
        }
        else if (tagInfo.IsEmpty)
        {
            await ShowAlert("Empty tag", title);
        }
        else
        {
            var first = tagInfo.Records[0];
            //await ShowAlert(GetMessage(first), title);
            var unitData = GetUnitData(first);
            CommitTransaction(unitData, _vm.Operation, _vm.Value);
            _vm.UnitData = unitData;
            await Publish(NFCNdefTypeFormat.Mime);
        }
    }

    /// <summary>
    /// Event raised when user cancelled NFC session on iOS 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Current_OniOSReadingSessionCancelled(object? sender, EventArgs e) => Debug("iOS NFC Session has been cancelled");

    /// <summary>
    /// Event raised when data has been published on the tag
    /// </summary>
    /// <param name="tagInfo">Published <see cref="ITagInfo"/></param>
    async void Current_OnMessagePublished(ITagInfo tagInfo)
    {
        try
        {
            CrossNFC.Current.StopPublishing();
            await ShowAlert("Writing tag operation successful");
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    /// <summary>
    /// Event raised when a NFC Tag is discovered
    /// </summary>
    /// <param name="tagInfo"><see cref="ITagInfo"/> to be published</param>
    /// <param name="format">Format the tag</param>
    async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
    {
        if (!CrossNFC.Current.IsWritingTagSupported)
        {
            await ShowAlert("Writing tag is not supported on this device");
            return;
        }

        try
        {
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

            CrossNFC.Current.PublishMessage(tagInfo, false);
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    private async void TransactionPage_Loaded(object? sender, EventArgs e)
    {
        // In order to support Mifare Classic 1K tags (read/write), you must set legacy mode to true.
        CrossNFC.Legacy = true;

        if (CrossNFC.IsSupported)
        {
            if (!CrossNFC.Current.IsAvailable)
                await ShowAlert("NFC is not available");

            _vm.IsNfcEnabled = CrossNFC.Current.IsEnabled;
            if (!_vm.IsNfcEnabled)
                await ShowAlert("NFC is disabled");

            if (DeviceInfo.Platform == DevicePlatform.iOS)
                _isDeviceiOS = true;

            //// Custom NFC configuration (ex. UI messages in French)
            //CrossNFC.Current.SetConfiguration(new NfcConfiguration
            //{
            //	DefaultLanguageCode = "fr",
            //	Messages = new UserDefinedMessages
            //	{
            //		NFCSessionInvalidated = "Session invalidée",
            //		NFCSessionInvalidatedButton = "OK",
            //		NFCWritingNotSupported = "L'écriture des TAGs NFC n'est pas supporté sur cet appareil",
            //		NFCDialogAlertMessage = "Approchez votre appareil du tag NFC",
            //		NFCErrorRead = "Erreur de lecture. Veuillez rééssayer",
            //		NFCErrorEmptyTag = "Ce tag est vide",
            //		NFCErrorReadOnlyTag = "Ce tag n'est pas accessible en écriture",
            //		NFCErrorCapacityTag = "La capacité de ce TAG est trop basse",
            //		NFCErrorMissingTag = "Aucun tag trouvé",
            //		NFCErrorMissingTagInfo = "Aucune information à écrire sur le tag",
            //		NFCErrorNotSupportedTag = "Ce tag n'est pas supporté",
            //		NFCErrorNotCompliantTag = "Ce tag n'est pas compatible NDEF",
            //		NFCErrorWrite = "Aucune information à écrire sur le tag",
            //		NFCSuccessRead = "Lecture réussie",
            //		NFCSuccessWrite = "Ecriture réussie",
            //		NFCSuccessClear = "Effaçage réussi"
            //	}
            //});

            SubscribeEvents();

            await StartListeningIfNotiOS();
        }
    }

    /// <summary>
    /// Task to publish data to the tag
    /// </summary>
    /// <param name="type"><see cref="NFCNdefTypeFormat"/></param>
    /// <returns>The task to be performed</returns>
    async Task Publish(NFCNdefTypeFormat? type = null)
    {
        try
        {
            _type = NFCNdefTypeFormat.Empty;

            if (type.HasValue) _type = type.Value;
            CrossNFC.Current.StartPublishing(!type.HasValue);
        }
        catch (Exception ex)
        {
            await ShowAlert(ex.Message);
        }
    }

    /// <summary>
    /// Returns the tag information from NDEF record
    /// </summary>
    /// <param name="record"><see cref="NFCNdefRecord"/></param>
    /// <returns>The tag information</returns>
    string GetMessage(NFCNdefRecord record)
    {
        var message = $"Message: {record.Message}";
        message += Environment.NewLine;
        message += $"RawMessage: {Encoding.UTF8.GetString(record.Payload)}";
        message += Environment.NewLine;
        message += $"Type: {record.TypeFormat}";

        if (!string.IsNullOrWhiteSpace(record.MimeType))
        {
            message += Environment.NewLine;
            message += $"MimeType: {record.MimeType}";
        }

        return message;
    }

    UnitData GetUnitData(NFCNdefRecord record)
    {
        var data = JsonSerializer.Deserialize<UnitData>(record.Message, _serializeOptions);
        if (data is null || !data.IsValidUnit())
            throw new KeyNotFoundException();

        return data;
    }

    /// <summary>
    /// Write a debug message in the debug console
    /// </summary>
    /// <param name="message">The message to be displayed</param>
    void Debug(string message) => System.Diagnostics.Debug.WriteLine(message);

    /// <summary>
    /// Display an alert
    /// </summary>
    /// <param name="message">Message to be displayed</param>
    /// <param name="title">Alert title</param>
    /// <returns>The task to be performed</returns>
    Task ShowAlert(string message, string? title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? ALERT_TITLE : title, message, "Cancel");

    /// <summary>
    /// Task to start listening for NFC tags if the user's device platform is not iOS
    /// </summary>
    /// <returns>The task to be performed</returns>
    async Task StartListeningIfNotiOS()
    {
        if (_isDeviceiOS)
            return;
        await BeginListening();
    }

    /// <summary>
    /// Task to safely start listening for NFC Tags
    /// </summary>
    /// <returns>The task to be performed</returns>
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

    /// <summary>
    /// Task to safely stop listening for NFC tags
    /// </summary>
    /// <returns>The task to be performed</returns>
    async Task StopListening()
    {
        try
        {
            CrossNFC.Current.StopListening();
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