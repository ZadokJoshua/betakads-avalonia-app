using System.IO;
using System.Diagnostics;
using FluentAvalonia.UI.Controls;

namespace Betakads.Helpers;

public static class HelperMethods
{
    private static IStorageProvider GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow?.StorageProvider is not { } provider)
        {
            throw new NullReferenceException("Missing StorageProvider instance.");
        }

        return provider;
    }

    public static async Task<IStorageItem?> OpenFilePickerAsync()
    {
        var files = await GetStorageProvider().OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select File",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.Pdf]
        });

        return files?.Count >= 1 ? files[0] : null;
    }

    public static async Task<IStorageItem?> OpenFolderPickerAsync()
    {
        var folder = await GetStorageProvider().OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select Folder",
            AllowMultiple = false
        });

        return folder?.Count >= 1 ? folder[0] : null;
    }

    public static void OpenAnkiImportSettings(string filePath)
    {
        string appDataPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).ToString();

        string executablePath = Path.Combine(appDataPath, @"Local\Programs\Anki\anki.exe");

        if (!File.Exists(executablePath))
        {
            throw new FileNotFoundException(string.Format("Executable '{0}' not found!", executablePath));
        }

        using Process ankiProcess = new();
        ankiProcess.StartInfo.FileName = executablePath;
        ankiProcess.StartInfo.Arguments = filePath;
        ankiProcess.Start();
    }

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

    private static readonly Dictionary<MessageBoxType, string> _messageBoxTitle = new()
    {
        { MessageBoxType.Error, "🚨 Error" },
        { MessageBoxType.Info, "ℹ Info" }
    };
}

public enum MessageBoxType
{
    Error,
    Info
}
