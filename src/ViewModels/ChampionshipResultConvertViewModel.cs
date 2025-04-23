using RaceResultConverter.Enum;
using RaceResultConverter.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RaceResultConverter.Conversion;

namespace RaceResultConverter.ViewModels;

public class ChampionshipResultConvertViewModel : INotifyPropertyChanged
{
    private readonly Func<ConvertType> _getConvertType;
    private readonly Action<bool, string> _setConvertResult;
    private string _championshipFolderPath = string.Empty;

    public string ChampionshipFolderPath
    {
        get => _championshipFolderPath;
        set
        {
            _championshipFolderPath = value;
            OnPropertyChanged(nameof(ChampionshipFolderPath));
        }
    }

    public ObservableCollection<ZRoundRaceResultItem> ZRoundRaceResultItems { get; set; } = new ObservableCollection<ZRoundRaceResultItem>();

    public ICommand SelectFolderCommand { get; set; }
    public RelayCommand ConvertCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ChampionshipResultConvertViewModel(Func<ConvertType> getConvertType, Action<bool, string> setConvertResult)
    {
        _getConvertType = getConvertType;
        _setConvertResult = setConvertResult;
        SelectFolderCommand = new RelayCommand(SelectFolder);
        ConvertCommand = new RelayCommand(Convert, CanConvert);
    }

    private void SelectFolder()
    {
        ZRoundRaceResultItems.Clear();

        var folderBrowserDialog = new FolderBrowserDialog
        {
                ShowNewFolderButton = false
            };
        if( folderBrowserDialog.ShowDialog() != DialogResult.OK) 
            return;

        ChampionshipFolderPath = folderBrowserDialog.SelectedPath;

        if(!Directory.Exists(ChampionshipFolderPath))
           return;

        var rcfResults = Directory.EnumerateFiles(ChampionshipFolderPath, "*.rcf", SearchOption.AllDirectories)
                               .Select(RcfFileParser.ParseZRoundResult);

        foreach (var zRoundRaceResultItem in rcfResults.Select(r => new ZRoundRaceResultItem
                 {
                     IsCheck = r.IsValid,
                     JsonFile = r.JsonFile,
                     RcfFile = r.ZRoundResult.Name,
                     ParseResult = r
                 }))
        {
            ZRoundRaceResultItems.Add(zRoundRaceResultItem);
        }

        ConvertCommand.RaiseCanExecuteChanged();
    }

    private void Convert()
    {
        var converter = new ZRoundResultConverter();
        var zonResults = ZRoundRaceResultItems.Where(result => result is { IsCheck: true, ParseResult.IsValid: true })
                                              .Select(result => converter.ConvertToTarget(result.ParseResult.ZRoundResult));

        var convertResult = zonResults.Aggregate(true, 
            (current, zonResult) => current & JsonUtil.SaveAsJson(zonResult, Path.ChangeExtension(Path.Combine(ChampionshipFolderPath, zonResult.Name), "json")));

        _setConvertResult(convertResult, convertResult ? Resource.ConvertResult_Succeed : Resource.ConvertResult_Failed);
    }

    private bool CanConvert() => ZRoundRaceResultItems?.Any(r => r is { IsCheck: true, ParseResult.IsValid: true }) ?? false;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}