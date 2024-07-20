using System.Diagnostics;

namespace Betakads.Helpers;

public static class AnkiHelper
{
    public static void OpenAnkiImportSettings(string filePath)
    {
        string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DirectoryInfo? appDataParentDirectory = Directory.GetParent(appDataFolderPath);

        if (appDataParentDirectory == null)
        {
            throw new DirectoryNotFoundException("The parent directory of the application data folder does not exist.");
        }

        string appDataPath = appDataParentDirectory.ToString();
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
}
