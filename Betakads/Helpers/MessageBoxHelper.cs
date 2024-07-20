using FluentAvalonia.UI.Controls;

namespace Betakads.Helpers;

public static class MessageBoxHelper
{
    private static readonly Dictionary<MessageBoxType, string> _messageBoxTitle = new()
        {
            { MessageBoxType.Error, "🚨 Error" },
            { MessageBoxType.Info, "ℹ Info" }
        };

    public static void ShowMessageBox(string message, MessageBoxType messageBoxType)
    {
        var resultHint = new ContentDialog()
        {
            Content = $"{message}",
            Title = _messageBoxTitle[messageBoxType],
            PrimaryButtonText = "Close"
        };

        _ = resultHint.ShowAsync();
    }
}

public enum MessageBoxType
{
    Error,
    Info
}
