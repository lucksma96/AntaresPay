namespace AntaresPay
{
    public class CurrencyBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object? sender, TextChangedEventArgs args)
        {
            if (sender is not Entry entry || string.IsNullOrEmpty(args.NewTextValue))
                return;

#if ANDROID
                var handler = entry.Handler as Microsoft.Maui.Handlers.EntryHandler;
                var editText = handler?.PlatformView as AndroidX.AppCompat.Widget.AppCompatEditText;
                if (editText != null)
                {
                    editText.EmojiCompatEnabled = false;
                    editText.SetTextKeepState(entry.Text);
                }
#endif

            // Remove any non-numeric characters from the text
            var newText = new string([.. args.NewTextValue.Where(char.IsDigit)]);

            // Convert the numeric text to a decimal value
            if (int.TryParse(newText, out var amount) && amount > 0)
            {
                // Format the decimal amount to the desired currency format
                string formattedAmount = $"₳ {amount:D}";

                // Set the formatted text to the Entry
                entry.Text = formattedAmount;
            }
            else
            {
                // If parsing fails, reset the Entry text
                entry.Text = string.Empty;
            }

            // Move the cursor to the end of the text
            entry.CursorPosition = entry.Text.Length;
        }
    }
}
