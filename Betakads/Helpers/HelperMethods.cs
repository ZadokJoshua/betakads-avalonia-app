using System.Diagnostics;

namespace Betakads.Helpers;

public static class HelperMethods
{
    public static async Task<IStorageItem?> DoOpenFileOrFolderPickerAsync(bool isFile)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop || desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        if (isFile)
        {
            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Select File",
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.Pdf }
            });

            return files?.Count >= 1 ? files[0] : null;
        }
        else
        {
            var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Select Folder",
                AllowMultiple = false
            });

            return folder?.Count >= 1 ? folder[0] : null;
        }
    }

    public static void OpenAnkiImportSettings(string filePath)
    {
        using Process ankiProcess = new();
        ankiProcess.StartInfo.FileName = @"C:\\Users\\Zadok\\AppData\\Local\\Programs\\Anki\\anki.exe";
        ankiProcess.StartInfo.Arguments = filePath;
        ankiProcess.Start();
    }
}
