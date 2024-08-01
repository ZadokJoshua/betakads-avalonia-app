using FluentAvalonia.UI.Controls;
using static Betakads.Helpers.StorageProviderHelper;
using static Betakads.Helpers.AnkiHelper;
using static Betakads.Helpers.MessageBoxHelper;
using Betakads.Models;

namespace Betakads.ViewModels;

public partial class MainViewModel(IPdfService pdfService, IYoutubeService youtubeService, IAIService aIService) : ViewModelBase
{
    #region Fields
    private readonly IPdfService _pdfService = pdfService;
    private readonly IYoutubeService _youtubeService = youtubeService;
    private readonly IAIService _aIService = aIService;

    [ObservableProperty]
    private string _fileName = "No File Selected";

    [ObservableProperty]
    private IStorageFile? _selectedFile;

    [ObservableProperty]
    private string _youtubeVideoUrl = string.Empty;

    [ObservableProperty]
    private bool _isSelectSourceTypeYoutube = true;

    [ObservableProperty]
    private int _numberOfcards;

    [ObservableProperty]
    private bool _changeFileNamePrefix;

    [ObservableProperty]
    private string _ankiTxtFilePrefix = string.Empty;

    [ObservableProperty]
    private string _extractedText = string.Empty;

    [ObservableProperty]
    private int _numberOfExtractedTextWords;

    [ObservableProperty]
    private int _numberOfExtractedTextChars;

    [ObservableProperty]
    private YoutubeMetadata? _youtubeMetadata;

    [ObservableProperty]
    private bool _isBusy;

    private string _savedFilePath = string.Empty;
    private readonly string _defaultFileName = $"Betakad-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";

    public ObservableCollection<Card> Cards { get; set; } = [];

    #endregion

    partial void OnExtractedTextChanged(string value)
    {
        NumberOfExtractedTextChars = Regex.Matches(value, "\\w").Count;
        NumberOfExtractedTextWords = Regex.Matches(value, "\\w+").Count;
    }

    private async Task<YoutubeMetadata> GetVideoMetaData(string videoUrl) =>
        await _youtubeService.GetVideoMetadata(videoUrl);

    private string ConvertGeneratedCardsToString()
    {
        StringBuilder ankiTxt = new();
        foreach (var card in Cards.ToList())
        {
            ankiTxt.AppendLine($"{card.Front};{card.Back}");
        }
        return ankiTxt.ToString();
    }

    #region Commands
    [RelayCommand]
    private async Task SelectFile()
    {
        var file = await OpenFilePickerAsync();
        if (file is null) return;
        SelectedFile = (IStorageFile)file;
        FileName = SelectedFile.Name;
    }

    [RelayCommand]
    private async Task ExtractText()
    {
        IsBusy = true;

        try
        {
            if (IsSelectSourceTypeYoutube && string.IsNullOrEmpty(YoutubeVideoUrl))
                throw new ArgumentNullException(nameof(YoutubeVideoUrl), "YoutubeVideoUrl cannot be null or empty.");
            if (!IsSelectSourceTypeYoutube && SelectedFile == null)
                throw new ArgumentNullException(nameof(SelectedFile), "SelectedFile cannot be null.");

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (IsSelectSourceTypeYoutube)
                {
                    ExtractedText = await _youtubeService.GetVideoCaptions(YoutubeVideoUrl);
                    YoutubeMetadata = await GetVideoMetaData(YoutubeVideoUrl);
                }
                else
                {
                    ExtractedText = await _pdfService.ExtractTxtFromPdf(SelectedFile.Path.LocalPath);
                }
            });
        }
        catch (Exception ex)
        {
            ShowMessageBox(ex.Message, Helpers.MessageBoxType.Error);
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
            if (string.IsNullOrWhiteSpace(ExtractedText)) throw new ArgumentNullException(nameof(ExtractedText), "Extracted text cannot be null or whitespace.");

            var cardsJson = await Dispatcher.UIThread
                .InvokeAsync(async () => await _aIService
                .ConvertTextToCardsList(new PromptPayload(ExtractedText, NumberOfcards)));

            if (string.IsNullOrEmpty(cardsJson)) return;

            List<Card>? cardsList = JsonSerializer.Deserialize<List<Card>>(cardsJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            Cards.Clear();

            foreach (var card in cardsList!)
            {
                Cards.Add(card);
            }
        }
        catch (Exception ex)
        {
            ShowMessageBox(ex.Message, Helpers.MessageBoxType.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void RemoveCardFromCollection(Card card) =>
        Cards.Remove(Cards.Where(i => i.CardId == card.CardId).Single());

    [RelayCommand]
    private async Task SaveAnkiCards()
    {
        if (Cards.Count <= 0)
        {
            ShowMessageBox("No generated cards!", Helpers.MessageBoxType.Info);
            return;
        }

        var selectedFolder = await OpenFolderPickerAsync();
        if (selectedFolder is not null)
        {
            string fileName = ChangeFileNamePrefix
                ? $"{AnkiTxtFilePrefix}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt"
                : _defaultFileName;

            _savedFilePath = Path.Combine(selectedFolder.Path.AbsolutePath, fileName);
            File.WriteAllText(_savedFilePath, ConvertGeneratedCardsToString());
        }
        ShowMessageBox("Cards saved!", Helpers.MessageBoxType.Info);
    }

    [RelayCommand]
    private void OpenInAnki()
    {
        if (Cards.Count <= 0)
        {
            ShowMessageBox("No generated cards!", Helpers.MessageBoxType.Info);
            return;
        }

        _savedFilePath = Path.Combine(Path.GetTempPath(), _defaultFileName);
        var txtString = ConvertGeneratedCardsToString();
        File.WriteAllText(_savedFilePath, txtString);
        try
        {
            OpenAnkiImportSettings(_savedFilePath);
        }
        catch (Exception ex)
        {
            ShowMessageBox(ex.Message, Helpers.MessageBoxType.Error);
        }
    }
    #endregion
}