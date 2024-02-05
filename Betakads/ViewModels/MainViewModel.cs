using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Models;
using static Betakads.Helpers.HelperMethods;

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

    public ObservableCollection<Card> Cards { get; set; } = [];
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

    private async Task<YoutubeMetadata> GetVideoMetaData(string videoUrl) => 
        await _youtubeService.GetVideoMetadata(videoUrl);

    #region Commands
    [RelayCommand]
    private async Task SelectFile()
    {
            var file = await DoOpenFileOrFolderPickerAsync(isFile: true);
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
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    if (IsSelectSourceTypeYoutube)
                    {
                        if (string.IsNullOrEmpty(YoutubeVideoUrl)) return;

                        ExtractedText = await _youtubeService.GetVideoCaptions(YoutubeVideoUrl);
                        YoutubeMetadata = await GetVideoMetaData(YoutubeVideoUrl);
                    }
                    else
                    {
                        if (SelectedFile == null) return;

                        ExtractedText = await _pdfService.ExtractTxtFromPdf(SelectedFile.Path.LocalPath);
                    }
                });
        }
        catch(Exception ex)
        {
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Error",
                ContentMessage = ex.Message,
                Icon = Icon.Error,
                MaxWidth = 500,
                MaxHeight = 800,
                ShowInCenter = true
            });

            await box.ShowAsync();
        }
        finally
        {
            IsBusy = false;
        }
       
    }

    [RelayCommand]
    private async Task GenerateCards()
    {
        if (string.IsNullOrWhiteSpace(ExtractedText)) return;

        IsBusy = true;

        try
        {
            var cardsJson = await Dispatcher.UIThread.InvokeAsync(async () => await _openAIService.ConvertTextToCardsList(ExtractedText, NumberOfcards));

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
            var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
            {
                ContentTitle = "Error",
                ContentMessage = ex.Message,
                Icon = Icon.Error,
                MaxWidth = 400,
                MaxHeight = 250,
                ShowInCenter = true,
                Topmost = false,
                ButtonDefinitions = [
                    new ButtonDefinition { Name = "Ok"},
                ]
            });

            await box.ShowAsync();
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

        var selectedFolder = await DoOpenFileOrFolderPickerAsync(isFile: false);

        if (selectedFolder != null)
        {
            foreach (var card in Cards.ToList())
            {
                ankiTxt.AppendLine($"{card.Front};{card.Back}");
            }

            string fileName = ChangeFileNamePrefix 
                ? $"{AnkiTxtFilePrefix}-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt" 
                : $"Betakad-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";

            string ankiFilePath = Path.Combine(selectedFolder.Path.AbsolutePath, fileName);
            File.WriteAllText(ankiFilePath, ankiTxt.ToString());
        }
    }
    #endregion
}