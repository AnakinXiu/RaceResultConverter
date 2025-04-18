using Microsoft.Win32;
using RaceResultConverter.Zon;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace RaceResultConverter;

public class SingleRaceConvertViewModel : INotifyPropertyChanged
{
    private string _selectedFilePath = string.Empty;
    private readonly OpenFileDialog _openFileDialog;
    private readonly Func<ConvertType> _getConvertType;
    private bool _convertResult;
    private string _convertMessage = string.Empty;

    public RelayCommand ConvertCommand { get; set; }

    public RelayCommand SelectFileCommand { get; set; }

    private bool CanConvert() => SourceFileSelected;

    private bool SourceFileSelected => !string.IsNullOrEmpty(SelectedFilePath) && File.Exists(SelectedFilePath);

    public ConvertType ConvertType => _getConvertType();

    public string SelectedFilePath
    {
        get => _selectedFilePath;
        set
        {
            _selectedFilePath = value;
            OnPropertyChanged(nameof(SelectedFilePath));
            ConvertCommand.RaiseCanExecuteChanged();
        }
    }

    public string ConvertMessage
    {
        get => _convertMessage;
        set
        {
            _convertMessage = value;
            OnPropertyChanged(nameof(ConvertMessage));
        }
    }

    public bool ConvertResult
    {
        get => _convertResult;
        set
        {
            _convertResult = value;
            OnPropertyChanged(nameof(ConvertResult));
        }
    }

    public SingleRaceConvertViewModel(Func<ConvertType> getConvertType)
    {
        _getConvertType = getConvertType;
        _openFileDialog = new OpenFileDialog
        {
            Filter = GetFileFilterString()
        };

        ConvertCommand = new RelayCommand(Convert, CanConvert);
        SelectFileCommand = new RelayCommand(SelectFile);
    }

    private string GetFileFilterString()
    {
        return ConvertType == ConvertType.ZRoundToZon ? "ZRound Race File|*.rcf" : "JSON File|*.json";
    }

    private static IRaceResult InnerConvert(string jsonFile)
    {
        return new ZRoundResultConverter(ConvertZRoundToZon).ConvertToTarget(jsonFile);
    }

    private static ZonResult ConvertZRoundToZon(ZRoundResult arg)
    {
        return new ZonResult
        {
            RaceDataProp = new RaceDataProp
            {
                RaceTitle = arg.Name,
                PrintTitle = arg.Name,
                Time = arg.Start,
                RaceRoundid = arg.Start,
                RaceMode = 1
            }
        };
    }

    private void Convert()
    {
        var result = InnerConvert(_openFileDialog.FileName);

        var saveFileDialog = new SaveFileDialog
        {
            AddExtension = true,
            CreatePrompt = true,
            FileName = "Result",
            DefaultExt = ".json",
            Filter = "JSON File|*.json",
            ValidateNames = true
        };

        if (saveFileDialog.ShowDialog() ?? false)
        {
            ConvertResult = JsonUtil.SaveAsJson(result, saveFileDialog.FileName);
            ConvertMessage = ConvertResult ? Resource.ConvertResult_Succeed : Resource.ConvertResult_Failed;

            return;
        }

        ConvertMessage = Resource.ConvertResult_Cancelled;
        ConvertResult = false;
    }


    private void SelectFile()
    {
        if (_openFileDialog.ShowDialog() ?? false)
            SelectedFilePath = _openFileDialog.FileName;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}