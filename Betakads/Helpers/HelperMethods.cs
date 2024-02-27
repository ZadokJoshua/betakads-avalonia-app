using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using System.Diagnostics;

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
            FileTypeFilter = new[] { FilePickerFileTypes.Pdf }
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
        using Process ankiProcess = new();
        ankiProcess.StartInfo.FileName = @"C:\\Users\\Zadok\\AppData\\Local\\Programs\\Anki\\anki.exe";
        ankiProcess.StartInfo.Arguments = filePath;
        ankiProcess.Start();
    }

    public static async Task ShowMessageBox(string message)
    {
        var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
        {
            ContentHeader = "🚨 Error",
            ContentMessage = message,
            MaxWidth = 400,
            MaxHeight = 700,
            SystemDecorations = Avalonia.Controls.SystemDecorations.None, // This removes the title bar
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
            Topmost = true,
            ShowInCenter = false,
            ButtonDefinitions = [
            new ButtonDefinition { Name = "Ok" },
            ]
        });


        await box.ShowAsync();
    }
}
