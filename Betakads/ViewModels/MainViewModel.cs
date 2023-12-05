using Betakads.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.IO;
using Betakads.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Avalonia.Threading;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System.Text;

namespace Betakads.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Fields
    private readonly PdfService _pdfService;
    private readonly YoutubeService _youtubeService;
    private readonly OpenAIService _openAIService;

    [ObservableProperty]
    private string _fileName = "No File Selected";

    [ObservableProperty]
    private IStorageFile? _selectedFile;

    [ObservableProperty]
    private string _youtubeVideoUrl = string.Empty;

    [ObservableProperty]
    private bool _isSelectSourceTypeYoutube = true;

    [ObservableProperty]
    private int _numberOfcards = 3;

    [ObservableProperty]
    private bool _changeFileNamePrefix;

    [ObservableProperty]
    private string __ankiTxtFilePrefix = string.Empty;

    [ObservableProperty]
    private string _extractedText = string.Empty;

    [ObservableProperty]
    private int _numberOfExtractedTextWords;

    [ObservableProperty]
    private int _numberOfExtractedTextChars;

    [ObservableProperty]
    private ObservableCollection<Card> _cards = new();

    [ObservableProperty]
    private YoutubeMetadata? _youtubeMetadata;

    [ObservableProperty]
    private bool _isBusy;
    #endregion

    public MainViewModel()
    {
        _pdfService = new();
        _youtubeService = new();
        _openAIService = new();
    }

    partial void OnExtractedTextChanged(string value)
    {
        NumberOfExtractedTextChars = Regex.Matches(value, "\\w").Count;
        NumberOfExtractedTextWords = Regex.Matches(value, "\\w+").Count;
    }

    private async Task<YoutubeMetadata> GetVideoMetaData(string videoUrl) => await _youtubeService.GetVideoMetadata(videoUrl);

    #region Commands
    [RelayCommand]
    private async Task SelectFile()
    {
            var file = await DoOpenFilePickerAsync();
            if (file is null) return;
            SelectedFile = file;
            FileName = SelectedFile.Name;
    }

    [RelayCommand]
    private async Task ExtractText()
    {
        IsBusy = true;
        try
        {
            if (IsSelectSourceTypeYoutube)
            {
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    ExtractedText = await _youtubeService.GetVideoCaptions(YoutubeVideoUrl);
                    YoutubeMetadata = await GetVideoMetaData(YoutubeVideoUrl);
                });
            }
            else
            {
                ExtractedText = _pdfService.ExtractTxtFromPdf(SelectedFile.Path.LocalPath);
            }
        }
        catch(Exception ex)
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Error", ex.Message,
              ButtonEnum.Ok, Icon.Error);

            var result = await box.ShowAsync();
        }
        finally
        {
            IsBusy = false;
        }
       
    }

    [RelayCommand]
    private async Task GenerateCards()
    {
        IsBusy = true;

        try
        {
            if (string.IsNullOrWhiteSpace(ExtractedText)) return;

            var cardsJson = await Dispatcher.UIThread.InvokeAsync(async () => await _openAIService.ConvertTextToCardsList(ExtractedText, NumberOfcards));

            if (string.IsNullOrEmpty(cardsJson)) return;

            List<Card> cardsList = JsonSerializer.Deserialize<List<Card>>(cardsJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            Cards.Clear();

            foreach (var card in cardsList)
            {
                Cards.Add(card);
            }
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager
          .GetMessageBoxStandard("Error", ex.Message,
              ButtonEnum.Ok, Icon.Error);

            var result = await box.ShowAsync();
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    private void RemoveCardFromCollection(Card card) => Cards.Remove(Cards.Where(i => i.Front == card.Front).Single());

    [RelayCommand]
    private async Task SaveAnkiCards()
    {
        if (Cards.Count <= 0) return;
        StringBuilder ankiTxt = new();

        var selectedFolder = await DoOpenFolderPickerAsync();

        if (selectedFolder != null)
        {
            foreach (var card in Cards.ToList())
            {
                ankiTxt.AppendLine($"{card.Front};{card.Back}");
            }

            string fileName = ChangeFileNamePrefix ? $"{AnkiTxtFilePrefix}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt" : $"Betakad-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
            string ankiFilePath = $@"{selectedFolder.Path.AbsolutePath}/{fileName}";
            File.WriteAllText(ankiFilePath, ankiTxt.ToString());
        }
    }
    #endregion

    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Select File",
            AllowMultiple = false,
            FileTypeFilter =  new[] { FilePickerFileTypes.Pdf }
        });

        return files?.Count >= 1 ? files[0] : null;
    }

    private async Task<IStorageFolder?> DoOpenFolderPickerAsync()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
                       desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        var folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            Title = "Select Folder",
            AllowMultiple = false
        });

        return folder?.Count >= 1 ? folder[0] : null;
    }
}
