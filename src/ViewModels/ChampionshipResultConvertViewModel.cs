using RaceResultConverter.Enum;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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
        ConvertCommand = new RelayCommand(Convert);
    }

    private void SelectFolder()
    {
        var folderBrowserDialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false
            };
        if( folderBrowserDialog.ShowDialog() != DialogResult.OK) 
            return;

        ChampionshipFolderPath = folderBrowserDialog.SelectedPath;

        if(!Directory.Exists(ChampionshipFolderPath))
           return;

        var enumerateFiles = Directory.EnumerateFiles(ChampionshipFolderPath, "*.rcf");
    }

    private void Convert()
    {
        // Implement conversion logic here
        // Use ChampionshipFolderPath to access the files
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}